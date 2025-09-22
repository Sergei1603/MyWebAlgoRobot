using Microsoft.AspNetCore.Mvc;

namespace CNYStrategy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CNYStrategyController : ControllerBase
    {

        private readonly ILogger<CNYStrategyController> _logger;

        public CNYStrategyController(ILogger<CNYStrategyController> logger)
        {
            _logger = logger;
        }
    }
}
