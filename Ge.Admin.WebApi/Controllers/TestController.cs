using Ge.Common;
using Ge.Infrastructure;
using Ge.Infrastructure.Attributes;
using Ge.Infrastructure.Caches;
using Ge.Infrastructure.Options;
using Ge.Model;
using Ge.Model.System.Dto;
using Ge.Repository.UnitOfWork;
using Ge.ServiceCore.Services;
using Ge.ServiceCore.Services.IServices;
using Infrastructure.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using SqlSugar;
using StackExchange.Profiling;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace Ge.Admin.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]    
    public class TestController : BaseController
    {

        private readonly ISysUserService _userService;

        public TestController(ISysUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("list")]
        public IActionResult List([FromQuery] SysUserQueryDto user, PagerInfo pager)
        {
            var list = _userService.SelectUserList(user, pager);

            return SUCCESS(list);
        }


    }
}
