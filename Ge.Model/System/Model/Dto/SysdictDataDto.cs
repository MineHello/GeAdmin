﻿namespace Ge.Model.System.Dto
{
    public class SysdictDataParamDto
    {
        public string DictType { get; set; }
        //public string ColumnName { get; set; }
        public List<SysDictDataDto> List { get; set; }
    }

    public class SysDictDataDto
    {
        public string DictLabel { get; set; }
        public string DictValue { get; set; }
        public string DictType { get; set; }
        public string CssClass { get; set; } = string.Empty;
        public string ListClass { get; set; } = string.Empty;
        /// <summary>
        /// 多语言翻译key值
        /// </summary>
        public string LangKey { get; set; } = string.Empty;
        /// <summary>
        /// 扩展1
        /// </summary>
        public string Extend1 { get; set; }
        /// <summary>
        /// 扩展2
        /// </summary>
        public string Extend2 { get; set; }
        #region uniapp返回字段
        public string Label { get; set; }
        public string Value { get; set; }
        #endregion
    }
}
