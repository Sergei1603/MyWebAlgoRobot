using MarketRobot.Interface;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace MarketRobot.Sheduler.Jobs.Sber
{
    internal class OpenPositionsSberJob : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public OpenPositionsSberJob(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var strategy = scope.ServiceProvider.GetService<IMarketSberStrategy>();
                var res = await strategy.OpenPosition();
            }
        }
    }
}
