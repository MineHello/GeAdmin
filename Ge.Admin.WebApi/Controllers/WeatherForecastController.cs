using Ge.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Ge.Admin.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public string Get()
        {
            return AppSettings.GetValue("name");
        }
    }
}
