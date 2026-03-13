/************************************************
 * Config class : 表格-表单
 * This file is can not be modify !!!
 ************************************************/

using System;
using System.Collections.Generic;

namespace GameConfig
{
    enum ItemType
    {
        Coin = 1,
        Diamond = 2,
        RemoveProp = 3,
        UndoProp = 4,
        RandomProp = 5,
        Photo = 6,
        Vip = 7,
        RemoveAd =8
    }
    public class Item
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string desc { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 物品初始数量
        /// </summary>
        public int initCount { get; set; }

        /// <summary>
        /// 是否货币
        /// </summary>
        public bool isCost { get; set; }

        /// <summary>
        /// 消耗金币数量
        /// </summary>
        public int coinprice { get; set; }

        /// <summary>
        /// 消耗钻石数量
        /// </summary>
        public int diamondprice { get; set; }
    }
}
