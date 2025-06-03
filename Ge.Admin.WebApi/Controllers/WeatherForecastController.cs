using Ge.Admin.WebApi.Extensions.AppExtensions;
using Ge.Infrastructure;
using Ge.Infrastructure.Options;
using Ge.Model;
using Ge.Repository.UnitOfWork;
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
        private readonly IUnitOfWorkManage uof;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IStudentService studentService, IUnitOfWorkManage uof)
        {
            _logger = logger;
            this.studentService = studentService;
            this.uof = uof;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public object Get()
        {


            Students students = new Students { StudentName = "zhangsan", GradeId = 1 };
            bool v = studentService.Insert(students);



            return v;
        }
    }
}
