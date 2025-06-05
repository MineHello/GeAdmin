using Ge.Admin.WebApi.Extensions.AppExtensions;
using Ge.Common;
using Ge.Infrastructure;
using Ge.Infrastructure.Attributes;
using Ge.Infrastructure.Options;
using Ge.Model;
using Ge.Repository.UnitOfWork;
using Ge.ServiceCore.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SqlSugar;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace Ge.Admin.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]    
    public class WeatherForecastController : ControllerBase
    {


        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IStudentService studentService;
        private readonly ISaleService saleService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IStudentService studentService,ISaleService saleService)
        {
            _logger = logger;
            this.studentService = studentService;
            this.saleService = saleService;
        }

        [Authorize("Permission")]
        [HttpGet(Name = "GetWeatherForecast")]        
        public object Get()
        {
            return saleService.GetList();
        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GetToken()
        {
            return JwtHelper.IssueJwt(new JwtHelper.TokenModelJwt { Uid = 1, Role = "admin",Work= "管理员" });

        }
    }
}
