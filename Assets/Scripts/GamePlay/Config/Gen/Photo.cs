/************************************************
 * Config class : 表格-表单
 * This file is can not be modify !!!
 ************************************************/

using System;
using System.Collections.Generic;

namespace GameConfig
{
    public class Photo
    {
        /// <summary>
        /// 图片名字
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 来源（0 普通 1 广告关 2 签到 3 新手礼包 4 商城礼包 5 选秀）
        /// </summary>
        public int sourceFrom { get; set; }

        /// <summary>
        /// BundleTag
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 图片品质 1是s 没有可以不写
        /// </summary>
        public int level { get; set; }

        /// <summary>
        /// 是否mp4
        /// </summary>
        public bool IsMP4 { get; set; }

        /// <summary>
        /// A包
        /// </summary>
        public bool IsShowA { get; set; }

        /// <summary>
        /// B包
        /// </summary>
        public bool IsShowB { get; set; }

        /// <summary>
        /// 其他信息
        /// </summary>
        public string other { get; set; }
    }
}