using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using MarketRobot.Interface;
using MarketRobot.Telegram;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace MarketRobot.Implementation
{
    internal class TinkoffProviderActive:ITinkoffProvider
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IOptions<InvestAPIOptions> _options;

        private InvestApiClient _client;
        public TinkoffProviderActive(IOptions<InvestAPIOptions> options)
        {
            _options = options;
        }

        public void SetClient(string name)
        {
            if(name == "SBER")
            {
                _client = InvestApiClientFactory.Create(_options.Value.InvestAPIKeySber);
            }
            else if(name == "CNY/RUB")
            {
                _client = InvestApiClientFactory.Create(_options.Value.InvestAPIKeyCN);
            }
        }

        public async Task<Future> GetFuturesByFigi(string figi)
        {
            return (await _client.Instruments.FutureByAsync(new InstrumentRequest()
            {
                IdType = InstrumentIdType.Figi,
                ClassCode = "SPBFUT",
                Id = figi
                //                Id = "SRU5"
            })).Instrument;
        }

        public async Task<string> GetFifiNearestFutures(string ticker)
        {
            return (await _client.Instruments.FuturesAsync()).Instruments
                .Where(x => x.BasicAsset == ticker)
                .OrderBy(x => x.LastTradeDate)
                .FirstOrDefault()?.Figi;
        }

        public async Task<RepeatedField<HistoricCandle>> GetCandles(string figi, DateTime from, DateTime to)
        {
            var res = (await _client.MarketData.GetCandlesAsync(new GetCandlesRequest()
            {
                CandleSourceType = GetCandlesRequest.Types.CandleSource.Exchange,
                From = Timestamp.FromDateTime(from),
                To = Timestamp.FromDateTime(to),
                Interval = CandleInterval._5Min,
                InstrumentId = figi,
            })).Candles;
            if (!res.Any())
            {
                Console.WriteLine($"Не удалось получить исторические свечи за период: от {from} до {to}");
                return null;
            }
            return res;
        }


        public async Task OpenPosition(string figi, bool direct, long quantity)//true - buy, false - sell
        {
            var acc = await _client.Users.GetAccountsAsync(new GetAccountsRequest()
            {
                Status = AccountStatus.Open,
            });
            var resp = await _client.Orders.PostOrderAsync(new PostOrderRequest()
            {
                AccountId = acc.Accounts[0].Id,
                ConfirmMarginTrade = true,
                Direction = direct ? OrderDirection.Buy : OrderDirection.Sell,
                InstrumentId = figi,
                OrderType = OrderType.Market,
                Quantity = quantity,
            });
        }

        public async Task ClosePosition(string figi)
        {
            var acc = await _client.Users.GetAccountsAsync(new GetAccountsRequest()
            {
                Status = AccountStatus.Open,
            });
            var positions = await _client.Operations.GetPositionsAsync(new PositionsRequest()
            {
                AccountId = acc.Accounts[0].Id,
            });
            var pos = positions.Futures.FirstOrDefault(x => x.Figi == figi);
            if (pos != null)
            {
                await OpenPosition(figi, pos.Balance < 0, pos.Balance);
            }
        }

        public async Task<decimal> GetBalance()
        {
            var acc = await _client.Users.GetAccountsAsync(new GetAccountsRequest()
            {
                Status = AccountStatus.Open,
            });
            var balance = (await _client.Operations.GetPortfolioAsync(new PortfolioRequest()
            {
                AccountId = acc.Accounts[0].Id,
                Currency = PortfolioRequest.Types.CurrencyRequest.Rub
            })).TotalAmountPortfolio;
            return balance;
        }

        public async Task OpneAccount()
        {
            //Не здесь)
        }

        public async Task<PositionsResponse> GetPositions()
        {
            var acc = await _client.Users.GetAccountsAsync(new GetAccountsRequest()
            {
                Status = AccountStatus.Open,
            });
            var res = await _client.Operations.GetPositionsAsync(new PositionsRequest()
            {
                AccountId = acc.Accounts[0].Id
            });
            return res;
        }
    }
}
