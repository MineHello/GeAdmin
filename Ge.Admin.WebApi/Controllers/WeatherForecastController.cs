using Ge.Admin.WebApi.Extensions.AppExtensions;
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
        

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public object Get()
        {
            RedisOptions redisOptions = App.GetOptions<RedisOptions>();
            return redisOptions;
        }
    }
}
