using Castle.DynamicProxy;
using Ge.Infrastructure.Attributes;
using Ge.Infrastructure.Caches;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ge.ServiceCore.AOP
{
    public class CacheAOP : IInterceptor
    {
        private readonly ICaching _cache;

        public CacheAOP(ICaching caching)
        {
            _cache = caching;
        }


        /// <summary>
        /// 自定义缓存的key
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        protected string CustomCacheKey(IInvocation invocation)
        {
            var typeName = invocation.TargetType.Name;
            var methodName = invocation.Method.Name;


            string key = $"{typeName}:{methodName}:";


            return key.TrimEnd(':');
        }




        //Intercept方法是拦截的关键所在，也是IInterceptor接口中的唯一定义
        public void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            //对当前方法的特性验证
            //如果需要验证
            var CachingAttribute = method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(CachingAttribute));
            if (CachingAttribute is CachingAttribute qCachingAttribute)
            {
                //获取自定义缓存键
                var cacheKey = CustomCacheKey(invocation);
                if (_cache.Exists(cacheKey))
                {
                    //将当前获取到的缓存值，赋值给当前执行方法
                    Type returnType;
                    if (typeof(Task).IsAssignableFrom(method.ReturnType))
                    {
                        returnType = method.ReturnType.GenericTypeArguments.FirstOrDefault();
                    }
                    else
                    {
                        returnType = method.ReturnType;
                    }

                    //根据key获取相应的缓存值
                    dynamic cacheValue = _cache.Get(returnType, cacheKey);
                    invocation.ReturnValue = (typeof(Task).IsAssignableFrom(method.ReturnType)) ? Task.FromResult(cacheValue) : cacheValue;
                    return;
                }

                //去执行当前的方法
                invocation.Proceed();
                //存入缓存
                if (!string.IsNullOrWhiteSpace(cacheKey))
                {
                    object response;

                    //Type type = invocation.ReturnValue?.GetType();
                    var type = invocation.Method.ReturnType;
                    if (typeof(Task).IsAssignableFrom(type))
                    {
                        dynamic result = invocation.ReturnValue;
                        response = result.Result;
                    }
                    else
                    {
                        response = invocation.ReturnValue;
                    }

                    if (response == null) response = string.Empty;

                    _cache.Set(cacheKey, response, TimeSpan.FromMinutes(qCachingAttribute.AbsoluteExpiration));
                }
            }
            else
            {
                invocation.Proceed(); //直接执行被拦截方法
            }
        }
    }
}
