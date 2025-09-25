using Microsoft.AspNetCore.Mvc;

namespace TelegrammBot.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TelegrammController : ControllerBase
    {

        private readonly ILogger<TelegrammController> _logger;

        public TelegrammController(ILogger<TelegrammController> logger)
        {
            _logger = logger;
        }

    }
}
