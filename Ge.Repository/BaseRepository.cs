using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ge.Repository
{
    public class BaseRepository<T> :  SimpleClient<T>,IBaseRepository<T> where T : class,new()
    {
        public ITenant itenant = null;//多租户事务

        public BaseRepository(ISqlSugarClient context = null):base(context) {
            var configId = typeof(T).GetCustomAttribute<TenantAttribute>()?.configId;
            if (configId != null)
            {
                Context = DbScoped.SugarScope.GetConnectionScope(configId);//根据类传入的ConfigId自动选择
            }
            else
            {
                Context = context ?? DbScoped.SugarScope.GetConnectionScope("main");//没有默认db0
            }
            //Context = DbScoped.SugarScope.GetConnectionScopeWithAttr<T>();
            itenant = DbScoped.SugarScope;//设置租户接口
        }

        
    }
}
