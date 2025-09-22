using Google.Protobuf.Collections;
using Tinkoff.InvestApi.V1;

namespace MarketRobot.Interface
{
    internal interface ITinkoffProvider
    {
        public Task<RepeatedField<HistoricCandle>> GetCandles(string ticker, DateTime from, DateTime to);
        public Task OpenPosition(string ticker, bool direct, long quantity);
        public Task<decimal> GetBalance();
        public Task<Future> GetFuturesByFigi(string figi);
        public Task ClosePosition(string ticker);
        public Task OpneAccount();
        public Task<PositionsResponse> GetPositions();
        public Task<string> GetFifiNearestFutures(string ticker);
        public void SetClient(string name);
    }
}
