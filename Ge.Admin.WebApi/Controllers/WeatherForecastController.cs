using Ge.Admin.WebApi.Extensions.AppExtensions;
using Ge.Infrastructure;
using Ge.Infrastructure.Options;
using Ge.ServiceCore.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SqlSugar;
using System.Text.Json.Serialization;

namespace Ge.Admin.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IStudentService studentService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,IStudentService studentService)
        {
            _logger = logger;
            this.studentService = studentService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public object Get()
        {
            return studentService.GetList();
        }
    }
}
