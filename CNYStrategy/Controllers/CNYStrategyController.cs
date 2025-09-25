using MarketRobot.Sheduler;
using Microsoft.AspNetCore.Mvc;

namespace CNYStrategy.Controllers
{
    [ApiController]
    [Route("CNY/[Controller]")]
    public class CNYStrategyController : ControllerBase
    {

        private readonly ILogger<CNYStrategyController> _logger;
        private readonly IServiceProvider _serviceProvider;

        public CNYStrategyController(ILogger<CNYStrategyController> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        [HttpGet(Name = "Start")]
        public void Start()
        {
            ShedulerTest.StartCNY(_serviceProvider);
        }

        [HttpGet(Name = "Stop")]
        public async void Stop()
        {
            await ShedulerTest.ShutDown();
        }
    }
}
