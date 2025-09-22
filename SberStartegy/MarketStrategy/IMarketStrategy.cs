using Google.Protobuf.Collections;
using Tinkoff.InvestApi.V1;

namespace MarketRobot.Interface
{
    internal interface IMarketStrategy
    {
        public Task ClosePosition();
        public Task<bool> OpenPosition();
        public Task<string> GetBalance();
    }

    internal struct CompareCandels
    {
        public RepeatedField<HistoricCandle> history;
        public RepeatedField<HistoricCandle> now;
    }

    internal struct TimePeriod
    {
        public DateTime from;
        public DateTime to;
    }
}
