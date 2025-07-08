using Ge.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar.Extensions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar.IOC;
using System.Reflection;
using Ge.Infrastructure.AppExtensions;
using System.Collections;

namespace Ge.ServiceCore.SqlSugar
{
    /// <summary>
    /// SqlSugar 启动服务
    /// </summary>
    public static class SqlsugarSetup
    {
        public static void AddSqlsugarSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // 默认添加主数据库连接
            if (!string.IsNullOrEmpty(AppSettings.app("MainDB")))
            {
                MainDb.CurrentDbConnId = AppSettings.app("MainDB");
            }

            BaseDBConfig.MutiConnectionString.allDbs.ForEach(m =>
            {
                var config = new IocConfig()
                {
                    ConfigId = m.ConnId.ObjToString().ToLower(),
                    ConnectionString = m.Connection,
                    DbType = (IocDbType)m.DbType,
                    IsAutoCloseConnection = true,

                };
                if (SqlSugarConst.LogConfigId.ToLower().Equals(m.ConnId.ToLower()))
                {
                    BaseDBConfig.LogConfig = config;
                }
                else
                {
                    BaseDBConfig.ValidConfig.Add(config);
                }

                BaseDBConfig.AllConfigs.Add(config);
            });

            if (BaseDBConfig.LogConfig is null)
            {
                throw new ApplicationException("未配置Log库连接");
            }

            // SqlSugarScope是线程安全，可使用单例注入
            // 参考：https://www.donet5.com/Home/Doc?typeId=1181
            //services.AddSingleton<ISqlSugarClient>(o =>
            //{
            //    return new SqlSugarScope(BaseDBConfig.AllConfigs);
            //});

            // SqlSugar ioc
            services.AddSqlSugar(BaseDBConfig.AllConfigs);




        }

        public static void InitTables(this IServiceCollection collection)
        {
            Type[] types = Assembly
        .LoadFrom(AppContext.BaseDirectory + "Ge.Model.dll")//如果 .dll报错，可以换成 xxx.exe 有些生成的是exe 
        .GetTypes().Where(it => it.FullName.Contains("Ge.Model"))//命名空间过滤，可以写其他条件
        .ToArray();//断点调试一下是不是需要的Type，不是需要的在进行过滤
            
         DbScoped.Sugar.GetConnectionScope(MainDb.CurrentDbConnId.ToLower()).CodeFirst.InitTables(types);
        }


    }

}
