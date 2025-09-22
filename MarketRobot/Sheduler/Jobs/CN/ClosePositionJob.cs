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
    internal class ClosePositionsCNJob : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public ClosePositionsCNJob(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var strategy = scope.ServiceProvider.GetService<IMarketCNStrategy>();
                await strategy.ClosePosition();
            }
        }
    }
}
