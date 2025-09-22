using MarketRobot.Interface;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace MarketRobot.Sheduler.Jobs.CN
{
    internal class OpenPositionsCNJob : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public OpenPositionsCNJob(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var strategy = scope.ServiceProvider.GetService<IMarketCNStrategy>();
                var res = await strategy.OpenPosition();
            }
        }
    }
}
