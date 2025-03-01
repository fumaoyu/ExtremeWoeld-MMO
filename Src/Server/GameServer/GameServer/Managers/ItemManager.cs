using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class ItemManager
    {
        /// <summary>
        /// 角色
        /// </summary>
        Character owner;

        /// <summary>
        /// 角色道具信息
        /// </summary>
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        public ItemManager(Character owner)
        {
            this.owner = owner;
            foreach (var item in owner.Data.Items)
            {
                this.Items.Add(item.ItemID, new Item(item));//把数据库中的道具信息保存到内存中
            }
        }

        public bool UseItem(int itemid, int count = 1)
        {
            Log.InfoFormat("[{0}] UserItem[ {1} : {2}]", this.owner.Data.ID, itemid, count);
            Item item = null;
            if (this.Items.TryGetValue(itemid, out item))//使用try为了性能,只有一次查询，还能取到
            {
                if (item.Count < count)
                    return false;

                //增加使用道具逻辑；
                item.Remove(count);//道具数量为零不从字典删除，后面玩的时候获得加上就行了
                return true;
            }
            return false;
        }
        /// <summary>
        /// 能否使用道具，数量是否够
        /// </summary>
        /// <param name="itemid"></param>
        /// <returns></returns>
        public bool HasItem(int itemid)
        {
            Item item = null;
            if (this.Items.TryGetValue(itemid, out item))
            {
                return item.Count > 0;
            }
            return false;
        }

        public Item GetItem(int itemid)
        {
            Item item = null;
            this.Items.TryGetValue(itemid, out item);
            Log.InfoFormat("[{0}] GetItem[{1} :{2}]", this.owner.Data.ID, itemid, item);
            return item;
        }

        public bool AddItem(int itemid, int count)
        {
            Item item = null;
            if (Items.TryGetValue(itemid,out item))
            {
                Items[itemid].Add(count);
            }
            else
            {
                TCharacterItem dbitem = new TCharacterItem();//数据库插入新道具信息
                dbitem.TCharacterID = owner.Data.ID;
                dbitem.ItemID = itemid;
                dbitem.ItemCount = count;
                owner.Data.Items.Add(dbitem);//插入db 数据库

                item = new Item(dbitem);
                //this.owner
                Items.Add(itemid,item);//插入内存字典
            }
            this.owner.StatusManager.AddItemChange(itemid, count, StatusAction.Add);//变化记录到状态管理器
            Log.InfoFormat("[{0}] AddItem[ ID:{1} Count:{2}]", this.owner.Data.ID, item, count);
            //DBService.Instance.Save();
            return true;

        }

        public bool RemoveItem(int itemid,int count)
        {
            Item item = null;
            if (Items.TryGetValue(itemid, out item))//这里不同和课程
            {
                if (item.Count < count)
                    return false;
                item.Remove(count);
                Log.InfoFormat("[{0} RemoveItem[ {1} :{2}]]", this.owner.Data.ID, item, count);
                //DBService.Instance.Save();
                this.owner.StatusManager.AddItemChange(itemid, count, StatusAction.Delete);////
                return true;
            }
            return false;
        }

        public void GetItemInfos(List<NItemInfo> list)//内存数据转换为网络数据
        {
            foreach (var item in this.Items)
            {
                list.Add(new NItemInfo() { Id = item.Value.ItemID, Count = item.Value.Count });
            }
        }
    }
}
