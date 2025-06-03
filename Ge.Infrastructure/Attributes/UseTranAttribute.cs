

using Ge.Infrastructure.Attribute;

namespace Ge.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class UseTranAttribute : System.Attribute
    {
        /// <summary>
        /// 事务传播方式
        /// </summary>
        public Propagation Propagation { get; set; } = Propagation.Required;
    }
}
