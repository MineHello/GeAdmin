﻿using SqlSugar;
using System;
using System.Linq;

namespace Ge.Model.System.Generate
{
    /// <summary>
    /// 代码生成表字段
    /// </summary>
    [SugarTable("gen_table_column", "代码生成表字段")]
    [Tenant("0")]
    public class GenTableColumn : SysBase
    {
        /// <summary>
        /// 列id
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public long ColumnId { get; set; }
        /// <summary>
        /// 导入代码生成表列名 首字母转了小写
        /// </summary>
        public string ColumnName { get; set; }
        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        public long TableId { get; set; }

        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        public string TableName { get; set; }
        /// <summary>
        /// 列说明
        /// </summary>
        private string columnComment;
        public string ColumnComment
        {
            get
            {
                return string.IsNullOrEmpty(columnComment) ? CsharpField : columnComment;
            }
            set
            {
                columnComment = value;
            }
        }
        /// <summary>
        /// 数据库列类型
        /// </summary>

        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        public string ColumnType { get; set; }
        /// <summary>
        /// C#类型
        /// </summary>
        public string CsharpType { get; set; }
        /// <summary>
        /// C# 字段名 首字母大写
        /// </summary>
        public string CsharpField { get; set; }
        /// <summary>
        /// 是否主键（1是）
        /// </summary>
        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        public bool IsPk { get; set; }
        /// <summary>
        /// 是否必填（1是）
        /// </summary>
        public bool IsRequired { get; set; }
        /// <summary>
        /// 是否自增（1是）
        /// </summary>
        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        public bool IsIncrement { get; set; }
        /// <summary>
        /// 是否插入（1是）
        /// </summary>
        public bool IsInsert { get; set; }
        /// <summary>
        /// 是否需要编辑（1是）
        /// </summary>
        public bool IsEdit { get; set; }
        /// <summary>
        /// 是否显示列表（1是）
        /// </summary>
        public bool IsList { get; set; }
        /// <summary>
        /// 是否查询（1是）
        /// </summary>
        public bool IsQuery { get; set; }
        /// <summary>
        /// 是否排序（1是）
        /// </summary>
        public bool IsSort { get; set; }
        /// <summary>
        /// 是否导出（1是）
        /// </summary>
        public bool IsExport { get; set; }
        /// <summary>
        /// 显示类型（文本框、文本域、下拉框、复选框、单选框、日期控件）
        /// </summary>
        public string HtmlType { get; set; }
        /// <summary>
        /// 查询类型（等于、不等于、大于、小于、范围）
        /// </summary>
        [SugarColumn(DefaultValue = "EQ")]
        public string QueryType { get; set; } = "EQ";
        public int Sort { get; set; }
        /// <summary>
        /// 字典类型
        /// </summary>
        public string DictType { get; set; } = "";
        /// <summary>
        /// 自动填充类型 1、添加 2、编辑 3、添加编辑
        /// </summary>
        public int AutoFillType { get; set; }

        #region 额外字段
        [SugarColumn(IsIgnore = true)]
        public string RequiredStr
        {
            get
            {
                string[] arr = new string[] { "int", "long" };
                return (!IsRequired && HtmlType != "selectMulti" && (arr.Any(f => f.Contains(CsharpType))) || typeof(DateTime).Name == CsharpType) ? "?" : "";
            }
        }
        /// <summary>
        /// 前端排序字符串
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string SortStr
        {
            get
            {
                return IsSort ? " sortable" : "";
            }
        }
        /// <summary>
        /// C# 字段名 首字母小写，用于前端
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string CsharpFieldFl
        {
            get
            {
                return CsharpField[..1].ToLower() + CsharpField[1..];
            }
        }
        /// <summary>
        /// 前端 只读字段
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string DisabledStr
        {
            get
            {
                return ((IsPk) && !IsRequired) ? " :disabled=\"true\"" : "";
            }
        }

        #endregion
    }
}
