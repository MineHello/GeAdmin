using Ge.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ge.ServiceCore
{
    public interface IBaseService<T> : IBaseRepository<T> where T : class,new()
    {
    }
}
