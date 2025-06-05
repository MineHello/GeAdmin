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

//�滻Autofac����
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

//��ʼ����
//builder.Services.InitTables();


#region jwt
// JWT
///�����֤toekn
var symmetricKeyAsBase64 = "aihsduiogaiusjnicoaschuoiasucs561612313";
var keyByteArray = Encoding.UTF8.GetBytes(symmetricKeyAsBase64);
var signingKey = new SymmetricSecurityKey(keyByteArray);
// ������֤����
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = signingKey,
    ValidateIssuer = true,
    ValidIssuer = "wg",//������
    ValidateAudience = true,
    ValidAudience = "wg",//������
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
//��֤
app.UseAuthentication();
//��Ȩ
app.UseAuthorization();

app.MapControllers();

app.Run();
