using Ge.Model;
using Ge.ServiceCore.Services.IServices;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ge.ServiceCore.Services
{
    public class StudentService : BaseService<Students>, IStudentService
    {
        public StudentService(ISqlSugarClient db) : base(db)
        {
        }
    }
}
