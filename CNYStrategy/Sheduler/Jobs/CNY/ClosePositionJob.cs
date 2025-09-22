using MarketRobot.Interface;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketRobot.Sheduler.Jobs.CN
{
    internal class ClosePositionsCNYJob : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public ClosePositionsCNYJob(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var strategy = scope.ServiceProvider.GetService<IMarketCNYStrategy>();
                await strategy.ClosePosition();
            }
        }
    }
}
