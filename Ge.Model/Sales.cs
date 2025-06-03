using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ge.Model
{
    [SqlSugar.SugarTable("Sales")]
    [Tenant("zr")]
    public class Sale
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
        public int SaleID { get; set; }
        public DateTime SaleDate { get; set; }

        public decimal Amount { get; set; }
    }
}
