using Ge.Repository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ge.ServiceCore
{
    public class BaseService<T> : BaseRepository<T>, IBaseService<T> where T : class, new()
    {
        
    }
}
