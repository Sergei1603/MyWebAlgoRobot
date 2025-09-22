using Google.Protobuf.WellKnownTypes;
using MarketRobot.Interface.Logger;
using Tinkoff.InvestApi.V1;
using static MarketRobot.Interface.IMarketStrategy;

namespace MarketRobot.Interface
{
    internal abstract class MarketStrategyAbstraction : IMarketStrategy
    {
        protected ITinkoffProvider _tinkoffProvider;
        protected ILogger _logger;

        protected string _basiсAktive; // SBER  CNY/RUB
        protected string _figi;
        protected int _value;
        protected bool _isPositionOpen = true;
        protected TimeZoneInfo _moscowZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");

        public async Task ClosePosition()
        {
            try
            {
                _logger.LogWithTG("Попытка закрытия позиции");
                var pos = await _tinkoffProvider.GetPositions();
                if (pos.Futures.Any())
                {
                    var futurePos = pos.Futures.FirstOrDefault();
                    await _tinkoffProvider.OpenPosition(_figi, futurePos.Balance < 0, Math.Abs(futurePos.Balance));
                    _isPositionOpen = false;

                    _logger.LogWithTG("Позиция закрыта");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWithTG("Произошла ошибка при закрытии позиции\n" +
                    ex.Message);
            }
        }
        public abstract Task<bool> OpenPosition();

        public async Task<string> GetBalance() => (await _tinkoffProvider.GetBalance()).ToString();

        protected decimal SMA(IEnumerable<HistoricCandle> candles) => candles.Average(x => x.Close);

        protected async Task SetValue(IEnumerable<HistoricCandle> nowCandels)
        {
            var balance = await _tinkoffProvider.GetBalance();
            var nowSMA = SMA(nowCandels);
            var future = await _tinkoffProvider.GetFuturesByFigi(_figi);
            var futurePrice = nowSMA / future.MinPriceIncrement * future.MinPriceIncrementAmount;
            _value = (int)Math.Round(balance / futurePrice, 0);
        }

        protected async Task SetFutureFigi() => _figi = await _tinkoffProvider.GetFifiNearestFutures(_basiсAktive);

        protected abstract TimePeriod SetDate();

        protected async Task<TimePeriod> PrepareSet()
        {
            await SetFutureFigi();
            return SetDate();
        }

        protected async Task<CompareCandels> GetCandels(TimePeriod timePeriod)
        {
            var from = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddMinutes(-10), _moscowZone);
            var to = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _moscowZone);

            var historyCandels = await _tinkoffProvider.GetCandles(_figi, timePeriod.from, timePeriod.to);
            var nowCandels = await _tinkoffProvider.GetCandles(_figi, TimeZoneInfo.ConvertTimeToUtc(from, _moscowZone),
        TimeZoneInfo.ConvertTimeToUtc(to, _moscowZone));
            return new CompareCandels()
            {
                history = historyCandels,
                now = nowCandels
            };
        }

        protected async Task Init()
        {
            var timePeriod = await PrepareSet();

            var compareCandels = await GetCandels(timePeriod);
            var historyCandels = compareCandels.history;
            var nowCandels = compareCandels.now;
            if (nowCandels is not null)
            {
                await SetValue(nowCandels);
            }
        }
    }
}
