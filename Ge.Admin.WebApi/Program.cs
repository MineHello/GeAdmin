using AspNetCoreRateLimit;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Ge.Admin.WebApi.Extensions;
using Ge.Admin.WebApi.Extensions.AotuFac;
using Ge.Admin.WebApi.Extensions.AppExtensions;
using Ge.Admin.WebApi.Extensions.CustomerAuth;
using Ge.Infrastructure;
using Ge.Infrastructure.Options;
using Ge.Model;
using Ge.Repository;
using Ge.ServiceCore;
using Ge.ServiceCore.Redis;
using Ge.ServiceCore.SqlSugar;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Filters;
using StackExchange.Profiling.Storage;
using System.Configuration;
using System.Reflection;
using System.Security.Claims;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// 读取配置
var configuration = builder.Configuration;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();



//替换Autofac容器
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule<AutoFacMoudleRegister>();
    }).ConfigureAppConfiguration((hostingContext, config) =>
    {
        hostingContext.Configuration.ConfigureApplication();
    });

builder.Host.UseSerilog();

builder.ConfigureApplication();


#region Add services to the container.
// Add services to the container.
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
builder.Services.AddSingleton(new AppSettings(builder.Configuration));
builder.Services.AddAllOptionRegister();
builder.Services.AddSqlsugarSetup();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.ConfigurationSugar(db =>
{
    db.GetConnection("main").Aop.OnLogExecuting = (sql, p) =>
    {
        Console.WriteLine(sql);
    };
    db.GetConnection("zr").Aop.OnLogExecuting = (sql, p) =>
    {
        Console.WriteLine(sql);
    };
});
builder.Services.AddCacheSetup();
#endregion

////初始化表
//builder.Services.InitTables();



#region IpRateLimit
IServiceCollection services = builder.Services;
Microsoft.Extensions.Configuration.ConfigurationManager configuration1 = builder.Configuration;

// 必需的服务
services.AddOptions();
services.AddMemoryCache();

// 配置限流选项
services.Configure<IpRateLimitOptions>(configuration1.GetSection("IpRateLimiting"));
services.Configure<IpRateLimitPolicies>(configuration1.GetSection("IpRateLimitPolicies"));

// 注册限流服务
services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

services.AddControllers();
#endregion

#region jwt
// JWT
///添加认证toekn
///

string iss = AppSettings.app(new string[] { "Audience", "Issuer" });
string aud = AppSettings.app(new string[] { "Audience", "Audience" });
string secret = AppSettings.app(new string[] { "Audience", "Secret" });

var symmetricKeyAsBase64 = secret;
var keyByteArray = Encoding.UTF8.GetBytes(symmetricKeyAsBase64);
var signingKey = new SymmetricSecurityKey(keyByteArray);
// 令牌验证参数
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = signingKey,
    ValidateIssuer = true,
    ValidIssuer = iss,//发行人
    ValidateAudience = true,
    ValidAudience = aud,//订阅人
    ValidateLifetime = true,
    ClockSkew = TimeSpan.FromSeconds(30),
    RequireExpirationTime = true,
};

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = tokenValidationParameters;
    });

builder.Services.AddSwaggerGen(c =>
{
    //添加Jwt验证设置,添加请求头信息
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                },
                new List<string>()
            }
        });

    //放置接口Auth授权按钮
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Value Bearer {token}",
        Name = "Authorization",//jwt默认的参数名称
        In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
        Type = SecuritySchemeType.ApiKey
    });

    ////就是这里！！！！！！！！！
    var xmlPath = Path.Combine(AppContext.BaseDirectory, "SwaggerDoc.xml");//这个就是刚刚配置的xml文件名
    c.IncludeXmlComments(xmlPath, true);//默认的第二个参数是false，这个是controller的注释，记得修改

});

#endregion

#region 自定义策略
var permission = new List<PermissionItem>();
var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
var permissionRequirement = new PermissionRequirement(
    "/api/denied",// 拒绝授权的跳转地址（目前无用）
    permission,//这里还记得么，就是我们上边说到的角色地址信息凭据实体类 Permission
    ClaimTypes.Role,//基于角色的授权
    iss,//发行人
    aud,//订阅人
    signingCredentials,//签名凭据
    expiration: TimeSpan.FromSeconds(60 * 2)//接口的过期时间，注意这里没有了缓冲时间，你也可以自定义，在上边的TokenValidationParameters的 ClockSkew
    );

builder.Services.AddAuthorization(c =>
{
    c.AddPolicy("Permission", p => p.Requirements.Add(permissionRequirement));
});

#endregion


#region MiniProfiler
builder.Services.AddMiniProfiler(options =>
{
    // 可选配置
    options.RouteBasePath = "/profiler"; // UI访问路径
    options.ColorScheme = StackExchange.Profiling.ColorScheme.Auto;
    options.EnableDebugMode = true; // 开发环境启用调试模式
    options.TrackConnectionOpenClose = true; // 跟踪数据库连接

});
#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseIpRateLimiting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        c =>
    {

        // 将swagger首页，设置成我们自定义的页面，记得这个字符串的写法：程序集名.index.html
        c.IndexStream = () => Assembly.GetExecutingAssembly().GetManifestResourceStream("Ge.Admin.WebApi.index.html");

    }
    );
}
//认证
app.UseAuthentication();
//授权
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();
app.UseMiniProfiler();


app.Run();
