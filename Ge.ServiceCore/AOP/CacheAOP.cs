using Castle.DynamicProxy;
using Ge.Infrastructure.Attributes;
using Ge.Infrastructure.Caches;
using Newtonsoft.Json;
using SqlSugar.Extensions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Ge.Infrastructure;

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
        /// AOP的拦截方法
        /// </summary>
        /// <param name="invocation"></param>

        /// <summary>
        /// 自定义缓存的key
        /// </summary>
        /// <param name="invocation"></param>
        /// <returns></returns>
        protected string CustomCacheKey(IInvocation invocation)
        {
            var typeName = invocation.TargetType.Name;
            var methodName = invocation.Method.Name;
            var methodArguments = invocation.Arguments.Select(GetArgumentValue).Take(3).ToList();//获取参数列表，最多三个

            string key = $"{typeName}:{methodName}:";
            foreach (var param in methodArguments)
            {
                key = $"{key}{param}:";
            }

            return key.TrimEnd(':');
        }

        /// <summary>
        /// object 转 string
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected static string GetArgumentValue(object arg)
        {
            if (arg is DateTime)
                return ((DateTime)arg).ToString("yyyyMMddHHmmss");

            if (arg != null  && arg != "")
                return arg.ObjToString();

            if (arg != null)
            {
                if (arg is Expression)
                {
                    var obj = arg as Expression;
                    var result = Resolve(obj);
                    return MD5Helper.MD5Encrypt16(result);
                }
                else if (arg.GetType().IsClass)
                {
                    return MD5Helper.MD5Encrypt16(JsonConvert.SerializeObject(arg));
                }

                return $"value:{arg.ObjToString()}";
            }
            return string.Empty;
        }

        private static string Resolve(Expression expression)
        {
            ExpressionContext expContext = new ExpressionContext();
            expContext.Resolve(expression, ResolveExpressType.WhereSingle);
            var value = expContext.Result.GetString();
            var pars = expContext.Parameters;

            pars.ForEach(s =>
            {
                value = value.Replace(s.ParameterName, s.Value.ObjToString());
            });

            return value;
        }

        private static string GetOperator(ExpressionType expressiontype)
        {
            switch (expressiontype)
            {
                case ExpressionType.And:
                    return "and";
                case ExpressionType.AndAlso:
                    return "and";
                case ExpressionType.Or:
                    return "or";
                case ExpressionType.OrElse:
                    return "or";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                default:
                    throw new Exception($"不支持{expressiontype}此种运算符查找！");
            }
        }





        private static string In(MethodCallExpression expression, object isTrue)
        {
            var Argument1 = (expression.Arguments[0] as MemberExpression).Expression as ConstantExpression;
            var Argument2 = expression.Arguments[1] as MemberExpression;
            var Field_Array = Argument1.Value.GetType().GetFields().First();
            object[] Array = Field_Array.GetValue(Argument1.Value) as object[];
            List<string> SetInPara = new List<string>();
            for (int i = 0; i < Array.Length; i++)
            {
                string Name_para = "InParameter" + i;
                string Value = Array[i].ToString();
                SetInPara.Add(Value);
            }
            string Name = Argument2.Member.Name;
            string Operator = Convert.ToBoolean(isTrue) ? "in" : " not in";
            string CompName = string.Join(",", SetInPara);
            string Result = $"{Name} {Operator} ({CompName})";
            return Result;
        }
        private static string Like(MethodCallExpression expression)
        {

            var Temp = expression.Arguments[0];
            LambdaExpression lambda = Expression.Lambda(Temp);
            Delegate fn = lambda.Compile();
            var tempValue = Expression.Constant(fn.DynamicInvoke(null), Temp.Type);
            string Value = $"%{tempValue}%";
            string Name = (expression.Object as MemberExpression).Member.Name;
            string Result = $"{Name} like {Value}";
            return Result;
        }


        private static string Len(MethodCallExpression expression, object value, ExpressionType expressiontype)
        {
            object Name = (expression.Arguments[0] as MemberExpression).Member.Name;
            string Operator = GetOperator(expressiontype);
            string Result = $"len({Name}){Operator}{value.ToString()}";
            return Result;
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
