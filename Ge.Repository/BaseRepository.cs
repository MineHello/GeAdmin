using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ge.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class,new()
    {

    }
}
