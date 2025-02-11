using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    class Item
    {
        public int Id;
        public int Count;

        public ItemDefine Define;
        public Item(NItemInfo item)//网络协议中的iteminfo ,因为客户端不接触db数据库
        {
            this.Id = item.Id;
            this.Count = item.Count;
            this.Define=DataManager.Instance.Items[item.Id];
        }

        public override string ToString()
        {
            return string.Format("ID：{0}，Count: {1}", this.Id, this.Count);
        }
    }
}
