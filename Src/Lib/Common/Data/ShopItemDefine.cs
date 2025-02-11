using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data
{
    /// <summary>
    /// 购买方式金币钻石还是都可以
    /// </summary>
   public enum PayType
    {
        Gold,
        Diamond,
        All
    }
    public class ShopItemDefine
    {
       // public int ShopItemID { get; set; }
        public int ItemID { get; set; }
        public int Count { get; set; }
        public PayType PayType { get; set; }
        public int GoldPrice { get; set; }
        public int DiacondPrice { get; set; }
        public int Status { get; set; }
    }
}
