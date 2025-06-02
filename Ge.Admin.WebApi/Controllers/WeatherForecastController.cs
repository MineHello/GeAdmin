using Ge.Infrastructure;
using Ge.Infrastructure.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace Ge.Admin.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IOptions<RedisOptions> options;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IOptions<RedisOptions> options)
        {
            _logger = logger;
            this.options = options;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public object Get()
        {            
            return options.Value;
        }
    }
}
