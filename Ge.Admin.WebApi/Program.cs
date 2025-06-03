using Autofac;
using Autofac.Extensions.DependencyInjection;
using Ge.Admin.WebApi.Extensions;
using Ge.Admin.WebApi.Extensions.AotuFac;
using Ge.Admin.WebApi.Extensions.AppExtensions;
using Ge.Infrastructure;
using Ge.Infrastructure.Options;
using Ge.Repository;
using Ge.ServiceCore;
using Ge.ServiceCore.SqlSugar;

var builder = WebApplication.CreateBuilder(args);

//Ìæ»»AutofacÈÝÆ÷
builder.Host
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
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

app.UseAuthorization();

app.MapControllers();

app.Run();
