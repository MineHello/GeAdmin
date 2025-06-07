using Autofac;
using Autofac.Extras.DynamicProxy;
using Ge.ServiceCore.AOP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ge.Admin.WebApi.Extensions.AotuFac
{
    public class AutoFacMoudleRegister : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var basePath = AppContext.BaseDirectory;

            var servicesDllFile = Path.Combine(basePath, "Ge.ServiceCore.dll");
            var repositoryDllFile = Path.Combine(basePath, "Ge.Repository.dll");

            var typeAops = new List<Type> { typeof(TranAOP), typeof(CacheAOP) };

            // 获取 Service.dll 程序集服务，并注册
            var assemblysServices = Assembly.LoadFrom(servicesDllFile);
            var assemblysRepository = Assembly.LoadFrom(repositoryDllFile);

            builder.RegisterType<TranAOP>();
            builder.RegisterType<CacheAOP>();

            builder.RegisterAssemblyTypes(assemblysServices)
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .PropertiesAutowired()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeAops.ToArray());//可以放一个AOP拦截器集合



            builder.RegisterAssemblyTypes(assemblysRepository)
                .AsImplementedInterfaces()
                .InstancePerDependency()
                .PropertiesAutowired();

        }
    }
}
