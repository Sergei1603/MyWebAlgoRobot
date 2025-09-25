using MarketRobot.Sheduler;
using Microsoft.AspNetCore.Mvc;

namespace SberStartegy.Controllers
{
    [ApiController]
    [Route("Sber/[Controller]")]
    public class SberController : ControllerBase
    {
        private readonly ILogger<SberController> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SberController(ILogger<SberController> logger, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        [HttpGet(Name = "Start")]
        public void Start()
        {
            ShedulerTest.StartSber(_serviceProvider);
        }

        [HttpGet(Name = "Stop")]
        public async void Stop()
        {
            await ShedulerTest.ShutDown();
        }
    }
}
