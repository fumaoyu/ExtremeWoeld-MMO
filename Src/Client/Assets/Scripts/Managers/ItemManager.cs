using Common.Data;
using Models;
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
