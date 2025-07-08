namespace Ge.Model.System.Model
{
    /// <summary>
    /// 文件分组
    /// </summary>
    [SugarTable("sys_file_group")]
    [Tenant(0)]
    public class SysFileGroup
    {
        /// <summary>
        /// id 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int GroupId { get; set; }

        /// <summary>
        /// 名称 
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 父级id 
        /// </summary>
        [SugarColumn(DefaultValue = "0")]
        public int ParentId { get; set; }

        /// <summary>
        /// 排序 
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否删除 
        /// </summary>
        public int? IsDelete { get; set; }

        /// <summary>
        /// 添加时间 
        /// </summary>
        [SugarColumn(InsertServerTime = true)]
        public DateTime? AddTime { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<SysFileGroup> Children { get; set; }
    }
}