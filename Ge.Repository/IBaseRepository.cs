﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ge.Repository
{
    public interface IBaseRepository<T> :ISimpleClient<T> where T : class,new()
    {

    }
}
