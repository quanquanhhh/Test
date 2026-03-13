/************************************************
 * Config class : 表格-表单
 * This file is can not be modify !!!
 ************************************************/

using System;
using System.Collections.Generic;

namespace GameConfig
{
    public class BaseGame
    {
        /// <summary>
        /// 好评关卡
        /// </summary>
        public int RateLevel { get; set; }

        /// <summary>
        /// 下载消耗
        /// </summary>
        public int DownloadCostType { get; set; }

        /// <summary>
        /// 下载消耗
        /// </summary>
        public int DownloadCostAmount { get; set; }

        /// <summary>
        /// 选关折扣
        /// </summary>
        public int ChoosePercent { get; set; }

        /// <summary>
        /// 选关刷新关卡数
        /// </summary>
        public int ChooseRefresh { get; set; }

        /// <summary>
        /// Loading进度条时间
        /// </summary>
        public int LoadingTime { get; set; }
    }
}
