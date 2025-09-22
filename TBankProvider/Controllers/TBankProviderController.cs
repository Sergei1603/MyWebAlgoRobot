using Microsoft.AspNetCore.Mvc;

namespace TBankProvider.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TBankProviderController : ControllerBase
    {

        private readonly ILogger<TBankProviderController> _logger;

        public TBankProviderController(ILogger<TBankProviderController> logger)
        {
            _logger = logger;
        }

    }
}
