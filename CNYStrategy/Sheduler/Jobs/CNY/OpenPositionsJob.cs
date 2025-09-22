using MarketRobot.Interface;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace MarketRobot.Sheduler.Jobs.CN
{
    internal class OpenPositionsCNYJob : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public OpenPositionsCNYJob(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var strategy = scope.ServiceProvider.GetService<IMarketCNYStrategy>();
                var res = await strategy.OpenPosition();
            }
        }
    }
}
