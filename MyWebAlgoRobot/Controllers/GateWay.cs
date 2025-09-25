using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Telegram.Bot.Types;

namespace MyWebAlgoRobot.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GateWay : ControllerBase
    {

        private readonly ILogger<GateWay> _logger;
        private readonly HttpClient _httpClient;

        public GateWay(ILogger<GateWay> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        [HttpGet("/Sber")]
        public async void StartSber()
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            var response = await _httpClient.GetAsync($"http://localhost:5261/Sber/Start");
        }

        [HttpGet("/CNY")]
        public async void StartCNY()
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            var response = await _httpClient.GetAsync($"http://localhost:5102/CNY/Start");
        }
    }
}
