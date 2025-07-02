using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ge.Model
{
    public class Students
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
        public int StudentId { get; set; }
        /// <summary>
        /// 学生姓名
        /// </summary>
        public string StudentName { get; set; }

        public int GradeId { get; set; }

        [SugarColumn(IsNullable = true)]
        public string? Remark { get; set; }

    }
}
