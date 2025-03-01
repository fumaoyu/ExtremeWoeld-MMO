using Common.Data;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// 客户端单例，服务端不是单例，因为服务器要管理所有角色，客户端只有自己的
    /// </summary>
    class ItemManager : Singleton<ItemManager>
    {

        public Dictionary<int, Item> Items = new Dictionary<int, Item>();
        internal void Init(List <NItemInfo>  items)//从服务器网络发过来的道具信息填充到内存
        {
            this.Items.Clear();
            foreach (var info in items)
            {
                Item item = new Item(info);
                this.Items.Add(item.Id, item);

                Debug.LogFormat("ItemManager:Init[{0}]", item);

            }
            StatusService.Instance.RegisterStatusNofity(StatusType.Item, OnItemNotify);
        }

        /// <summary>
        /// 訂閲了消息，
        /// </summary>+
        /// <param name="status"></param>
        /// <returns></returns>
        private bool OnItemNotify(NStatus status)
        {
            if (status.Action == StatusAction.Add)
            {
                this.AddItem(status.Id, status.Value);
            }
            if (status.Action == StatusAction.Delete)
            {
                this.RemoveItem(status.Id, status.Value);
            }
            return true;
        }

        private void AddItem(int id, int count)
        {
            Item item = null;
            if (Items.TryGetValue(id, out item))
            {
                item.Count += count;
            }
            else
            {
                item = new Item(id, count);
                Items.Add(id, item);
            }
            BagManager.Instance.AddItem(id, count);//道具更新了背包里面信息也要更新
        }
        private void RemoveItem(int id, int count)
        {
           if (!this.Items.ContainsKey(id))
            {
                return;
            }
           Item  item=this.Items[id];//
            if (item.Count < count)
            {
                return;
            }
            item.Count -= count;
            BagManager.Instance.RemoveItem(id, count);
        }

  

        public ItemDefine GetItem(int itemid)
        {
            return null;
        }

        public bool UseItem(int itemid)
        {
            return false;
        }
        public bool UseItem(ItemDefine item)
        {
            return false;
        }
    }
}
