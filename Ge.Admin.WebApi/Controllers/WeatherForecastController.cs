using Ge.Admin.WebApi.Extensions.AppExtensions;
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
            var iss = "wg";
            var aud = "wg";
            var secret = "aihsduiogaiusjnicoaschuoiasucs561612313";

            List<Claim> claims = new List<Claim>()
            {
               //uid
                new Claim(JwtRegisteredClaimNames.Jti, "0001"),
                new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                //这个就是过期时间，目前是过期1000秒，可自定义，注意JWT有自己的缓冲过期时间
                new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddSeconds(1000)).ToUnixTimeSeconds()}"),
                new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(1000).ToString()),
                new Claim(JwtRegisteredClaimNames.Iss,iss),
                new Claim(JwtRegisteredClaimNames.Aud,aud),
                new Claim(ClaimTypes.Role,"admin"),
                
            };


            //秘钥 (SymmetricSecurityKey 对安全性的要求，密钥的长度太短会报出异常)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: iss,
                claims: claims,
                signingCredentials: creds
            );

            var jwtHandler = new JwtSecurityTokenHandler();
            var encodedJwt = jwtHandler.WriteToken(jwt);

            return encodedJwt;
        }
    }
}
