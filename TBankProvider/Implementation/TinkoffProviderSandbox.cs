using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using MarketRobot.Interface;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace MarketRobot.Implementation
{
    internal class TinkoffProviderSandbox:ITinkoffProvider
    {
        private InvestApiClient _client;
        public TinkoffProviderSandbox(InvestApiClient client) 
        {
            _client = client;
        }

        public void SetClient(string name)
        {

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
            var res = _client.MarketData.GetCandles(new GetCandlesRequest()
            {
                CandleSourceType = GetCandlesRequest.Types.CandleSource.Exchange,
                From = Timestamp.FromDateTime(from),
                To = Timestamp.FromDateTime(to),
                Interval = CandleInterval._5Min,
                InstrumentId = figi,
            }).Candles;
            if (!res.Any())
            {
                Console.WriteLine($"Не удалось получить исторические свечи за период: от {from} до {to}");
                return null;
            }
            return res;
        }

        public async Task OpenPosition(string figi, bool direct, long quantity)//true - buy, false - sell
        {
            var acc = _client.Sandbox.GetSandboxAccounts(new GetAccountsRequest()
            {
                Status = AccountStatus.Open,
            });
            var resp = _client.Sandbox.PostSandboxOrder(new PostOrderRequest()
            {
                AccountId = acc.Accounts[0].Id,
                ConfirmMarginTrade = true,
                Direction = direct? OrderDirection.Buy : OrderDirection.Sell,
                InstrumentId = figi,
                OrderType = OrderType.Market,
                Quantity = quantity
            });
        }

        public async Task ClosePosition(string ticker)
        {
            var acc = _client.Sandbox.GetSandboxAccounts(new GetAccountsRequest()
            {
                Status = AccountStatus.Open,
            });
            var positions = _client.Sandbox.GetSandboxPositions(new PositionsRequest()
            {
                AccountId = acc.Accounts[0].Id,
            });
            var pos = positions.Futures.FirstOrDefault(x => x.Ticker == ticker);
            if (pos != null)
            {
                OpenPosition(ticker, pos.Balance < 0, pos.Balance);
            }
        }

        public async Task<decimal> GetBalance()
        {
            var acc = _client.Sandbox.GetSandboxAccounts(new GetAccountsRequest()
            {
                Status = AccountStatus.Open,
            });
            var balance = _client.Sandbox.GetSandboxPortfolio(new PortfolioRequest()
            {
                AccountId = acc.Accounts[0].Id,
                Currency = PortfolioRequest.Types.CurrencyRequest.Rub
            }).TotalAmountPortfolio;
            return balance;
        }

        public async Task OpneAccount()
        {
            var acc = _client.Sandbox.GetSandboxAccounts(new GetAccountsRequest()
            {
                Status = AccountStatus.Open,
            });
            var res = _client.Sandbox.SandboxPayIn(new SandboxPayInRequest()
            {
                AccountId = acc.Accounts[0].Id,
                //        AccountId = "a1c3df42-804b-4b7d-a74b-cf7ed0aa2b17",
                Amount = new MoneyValue() { Nano = 0, Currency = "RUB643", Units = 100000 },
            });
        }

        public async Task<PositionsResponse> GetPositions()
        {
            var acc = _client.Sandbox.GetSandboxAccounts(new GetAccountsRequest()
            {
                Status = AccountStatus.Open,
            });
            var res = _client.Sandbox.GetSandboxPositions(new PositionsRequest()
            {
                AccountId = acc.Accounts[0].Id
            });
            return res;
        }
    }
}
