using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Models
{
  public   class Item
    {
        public int Id;
        public int Count;
        public EquipDefine EquipInfo;//为了数据维护简单让装备和道具用同一个id,这里发一个道具id
        public ItemDefine Define;
        public Item(NItemInfo item):this(item.Id,item.Count)//网络协议中的iteminfo ,因为客户端不接触db数据库
        {
            //this.Id = item.Id;
            //this.Count = item.Count;
            //this.Define=DataManager.Instance.Items[item.Id];
        }
        public Item(int id,int count)
        {
            this.Id = id;
            this.Count = count;
             DataManager.Instance.Items.TryGetValue(this.Id,out this.Define);
            DataManager.Instance.Equips.TryGetValue(this.Id, out EquipInfo);

        }

        public override string ToString()
        {
            return string.Format("ID：{0}，Count: {1}", this.Id, this.Count);
        }
    }
}
