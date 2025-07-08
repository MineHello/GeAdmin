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

// ��ȡ����
var configuration = builder.Configuration;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();



//�滻Autofac����
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

////��ʼ����
//builder.Services.InitTables();



#region IpRateLimit
IServiceCollection services = builder.Services;
Microsoft.Extensions.Configuration.ConfigurationManager configuration1 = builder.Configuration;

// ����ķ���
services.AddOptions();
services.AddMemoryCache();

// ��������ѡ��
services.Configure<IpRateLimitOptions>(configuration1.GetSection("IpRateLimiting"));
services.Configure<IpRateLimitPolicies>(configuration1.GetSection("IpRateLimitPolicies"));

// ע����������
services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

services.AddControllers();
#endregion

#region jwt
// JWT
///�����֤toekn
///

string iss = AppSettings.app(new string[] { "Audience", "Issuer" });
string aud = AppSettings.app(new string[] { "Audience", "Audience" });
string secret = AppSettings.app(new string[] { "Audience", "Secret" });

var symmetricKeyAsBase64 = secret;
var keyByteArray = Encoding.UTF8.GetBytes(symmetricKeyAsBase64);
var signingKey = new SymmetricSecurityKey(keyByteArray);
// ������֤����
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = signingKey,
    ValidateIssuer = true,
    ValidIssuer = iss,//������
    ValidateAudience = true,
    ValidAudience = aud,//������
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
    //���Jwt��֤����,�������ͷ��Ϣ
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

    //���ýӿ�Auth��Ȩ��ť
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Value Bearer {token}",
        Name = "Authorization",//jwtĬ�ϵĲ�������
        In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
        Type = SecuritySchemeType.ApiKey
    });

    ////�����������������������
    var xmlPath = Path.Combine(AppContext.BaseDirectory, "SwaggerDoc.xml");//������Ǹո����õ�xml�ļ���
    c.IncludeXmlComments(xmlPath, true);//Ĭ�ϵĵڶ���������false�������controller��ע�ͣ��ǵ��޸�

});

#endregion

#region �Զ������
var permission = new List<PermissionItem>();
var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
var permissionRequirement = new PermissionRequirement(
    "/api/denied",// �ܾ���Ȩ����ת��ַ��Ŀǰ���ã�
    permission,//���ﻹ�ǵ�ô�����������ϱ�˵���Ľ�ɫ��ַ��Ϣƾ��ʵ���� Permission
    ClaimTypes.Role,//���ڽ�ɫ����Ȩ
    iss,//������
    aud,//������
    signingCredentials,//ǩ��ƾ��
    expiration: TimeSpan.FromSeconds(60 * 2)//�ӿڵĹ���ʱ�䣬ע������û���˻���ʱ�䣬��Ҳ�����Զ��壬���ϱߵ�TokenValidationParameters�� ClockSkew
    );

builder.Services.AddAuthorization(c =>
{
    c.AddPolicy("Permission", p => p.Requirements.Add(permissionRequirement));
});

#endregion


#region MiniProfiler
builder.Services.AddMiniProfiler(options =>
{
    // ��ѡ����
    options.RouteBasePath = "/profiler"; // UI����·��
    options.ColorScheme = StackExchange.Profiling.ColorScheme.Auto;
    options.EnableDebugMode = true; // �����������õ���ģʽ
    options.TrackConnectionOpenClose = true; // �������ݿ�����

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

        // ��swagger��ҳ�����ó������Զ����ҳ�棬�ǵ�����ַ�����д����������.index.html
        c.IndexStream = () => Assembly.GetExecutingAssembly().GetManifestResourceStream("Ge.Admin.WebApi.index.html");

    }
    );
}
//��֤
app.UseAuthentication();
//��Ȩ
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();
app.UseMiniProfiler();


app.Run();
