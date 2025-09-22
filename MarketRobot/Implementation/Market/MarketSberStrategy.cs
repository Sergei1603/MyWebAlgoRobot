using MarketRobot.Interface;
using MarketRobot.Interface.Logger;
using static MarketRobot.Interface.IMarketStrategy;

namespace MarketRobot.Implementation
{
    internal class MarketSberStrategy : MarketStrategyAbstraction, IMarketSberStrategy
    {


        public MarketSberStrategy(ITinkoffProvider tinkoffProvider, ILogger logger)
        {
            _tinkoffProvider = tinkoffProvider;
            _logger = logger;

            _basiсAktive = "SBER"; // CNY/RUB
            _tinkoffProvider.SetClient(_basiсAktive);

            Init().Wait();
        }

        protected override TimePeriod SetDate()
        {
            var _from = TimeZoneInfo.ConvertTimeToUtc(new DateTime(DateTime.Today.Year, 
                DateTime.Today.Month, DateTime.Today.Day, 10, 35, 0), _moscowZone);
            var _to = TimeZoneInfo.ConvertTimeToUtc(new DateTime(DateTime.Today.Year, 
                DateTime.Today.Month, DateTime.Today.Day, 10, 45, 0), _moscowZone);

            return new TimePeriod()
            {
                from = _from,
                to = _to,
            };
        }

        public override async Task<bool> OpenPosition()
        {
            try
            {
                _logger.LogWithTG("Начало открытия позиции");

                var timePeriod = await PrepareSet();

                var compareCandels = await GetCandels(timePeriod);
                var historyCandels = compareCandels.history;
                var nowCandels = compareCandels.now;

                if (historyCandels is not null && nowCandels is not null)
                {
                    await SetValue(nowCandels);
                    var historySMA = SMA(historyCandels);
                    var nowSMA = SMA(nowCandels);
                    var deltaPrice = (nowSMA - historySMA) / historySMA * 100;

                    _logger.LogConsole("Изменение цены: " + deltaPrice + "\n" +
                        await _tinkoffProvider.GetPositions());

                    if (_value > 0)
                    {
                        if (nowSMA > historySMA)
                        {
                            await _tinkoffProvider.OpenPosition(_figi, true, _value);
                            _isPositionOpen = true;
                            _logger.LogWithTG($"Позиция открыта в лонг\nИзменение цены: {deltaPrice}\n");
                            //лонг
                        }
                        else if (deltaPrice < -2)
                        {
                            await _tinkoffProvider.OpenPosition(_figi, false, _value);
                            _isPositionOpen = true;
                            _logger.LogWithTG($"Позиция открыта в шорт\nИзменение цены: {deltaPrice}\n");
                            //шорт
                        }
                        else
                        {
                            _logger.LogWithTG("Позиция не была открыта, не выполнено условие");
                        }
                    }
                }
     //           Console.WriteLine(await _tinkoffProvider.GetPositions());
                return _isPositionOpen;
            }
            catch(Exception ex)
            {
                _logger.LogWithTG("Произошла ошибка при открытии позиции\n" +
                    ex.Message);
                return false;
            }
        }
    }
}
