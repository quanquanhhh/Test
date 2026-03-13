/************************************************
 * Config class : 表格-表单
 * This file is can not be modify !!!
 ************************************************/

using System;
using System.Collections.Generic;

namespace GameConfig
{
    public class Sign
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 签到奖励类型（1：金币 2：钻石 3：图片）
        /// </summary>
        public int SignRewardType { get; set; }

        /// <summary>
        /// 签到奖励数量
        /// </summary>
        public int SignRewardNum { get; set; }
    }
}
