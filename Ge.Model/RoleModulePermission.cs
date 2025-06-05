using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ge.Model
{
    /// <summary>
    /// 按钮跟权限关联表
    /// </summary>
    public class RoleModulePermission 
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true)]
        public int Id { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }
        /// <summary>
        /// 菜单ID，这里就是api地址的信息
        /// </summary>
        public int ModuleId { get; set; }
        /// <summary>
        /// 按钮ID
        /// </summary>
        public int? PermissionId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        ///获取或设置是否禁用，逻辑上的删除，非物理删除
        /// </summary>
        public bool? IsDeleted { get; set; }

        // 等等，还有其他属性，其他的可以参考Code，或者自定义...

        // 请注意，下边三个实体参数，只是做传参作用，所以忽略下，不然会认为缺少字段
        [SugarColumn(IsIgnore = true)]
        public virtual Role Role { get; set; }
        [SugarColumn(IsIgnore = true)]
        public virtual Modules Module { get; set; }
        [SugarColumn(IsIgnore = true)]
        public virtual Permission Permission { get; set; }
    }
}
