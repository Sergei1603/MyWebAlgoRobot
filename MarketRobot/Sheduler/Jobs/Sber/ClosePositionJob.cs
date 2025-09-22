using MarketRobot.Interface;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace MarketRobot.Sheduler.Jobs.Sber
{
    internal class ClosePositionsSberJob:IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public ClosePositionsSberJob(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var strategy = scope.ServiceProvider.GetService<IMarketSberStrategy>();
                await strategy.ClosePosition();
            }
        }
    }
}
