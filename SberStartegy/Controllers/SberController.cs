using MarketRobot.Sheduler;
using Microsoft.AspNetCore.Mvc;

namespace SberStartegy.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
            Sheduler.StartSber(_serviceProvider);
        }

        [HttpGet(Name = "Start")]
        public async void Stop()
        {
            await Sheduler.ShutDown();
        }
    }
}
