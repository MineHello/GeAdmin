﻿using Ge.Model;
using Ge.Model.System;

namespace Ge.ServiceCore.Model.Dto
{
    public class SysPostDto : SysPost
    {
        /// <summary>
        /// 用户个数
        /// </summary>
        public long UserNum { get; set; }
    }

    public class SysPostQueryDto : PagerInfo
    {
        public string PostName { get; set; }
        public string Status { get; set; }
        public string PostCode { get; set; }
    }
}
