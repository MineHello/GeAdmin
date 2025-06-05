using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Ge.Admin.WebApi.Extensions;
using Ge.Admin.WebApi.Extensions.AotuFac;
using Ge.Admin.WebApi.Extensions.AppExtensions;
using Ge.Admin.WebApi.Extensions.CustomerAuth;
using Ge.Infrastructure;
using Ge.Infrastructure.Options;
using Ge.Repository;
using Ge.ServiceCore;
using Ge.ServiceCore.SqlSugar;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//替换Autofac容器
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule<AutoFacMoudleRegister>();
    }).ConfigureAppConfiguration((hostingContext, config) =>
    {
        hostingContext.Configuration.ConfigureApplication();
    });

builder.ConfigureApplication();


#region Add services to the container.
// Add services to the container.
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
builder.Services.AddSingleton(new AppSettings(builder.Configuration));
builder.Services.AddAllOptionRegister();
builder.Services.AddSqlsugarSetup();
builder.Services.AddScoped<IAuthorizationHandler, PermissionRequment>();
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
#endregion

//初始化表
//builder.Services.InitTables();


#region jwt
// JWT
///添加认证toekn
var symmetricKeyAsBase64 = "aihsduiogaiusjnicoaschuoiasucs561612313";
var keyByteArray = Encoding.UTF8.GetBytes(symmetricKeyAsBase64);
var signingKey = new SymmetricSecurityKey(keyByteArray);
// 令牌验证参数
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = signingKey,
    ValidateIssuer = true,
    ValidIssuer = "wg",//发行人
    ValidateAudience = true,
    ValidAudience = "wg",//订阅人
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

builder.Services.AddAuthorization(c =>
{
    c.AddPolicy("Permission",p => p.Requirements.Add(new PermissionRequment()));
});

#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseApplicationSetup();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//认证
app.UseAuthentication();
//授权
app.UseAuthorization();

app.MapControllers();

app.Run();
