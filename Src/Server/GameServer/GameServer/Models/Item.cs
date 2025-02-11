using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    /// <summary>
    /// 这个类就是做了一个封装,
    /// </summary>
    class Item
    {
        /// <summary>
        /// 数据库的道具，为了方便服务端，客户端通讯之间道具，一次性把数据库的道具读出来保存在内存中，后面需要道具信息读内存就行了，不需要每次都读数据库
        /// </summary>
        TCharacterItem dbItem;


        public int ItemID;

        public int Count;

        public Item(TCharacterItem item)
        {
            this.dbItem = item;

            this.Count = (short)item.ItemCount;
            this.ItemID = (short)item.ItemID;
        }

        public void Add(int count)
        {
            this.Count += count;
          
            dbItem.ItemCount = Count;
        }

        public void Remove(int count)
        {
            this.Count -= count;
            dbItem.ItemCount = Count;
        }
        /// <summary>
        /// 是否可以使用
        /// </summary>
        /// <returns></returns>
        public bool Use(int count=1)
        {
            return false;
        }

        /// <summary>
        /// 重载简化输出
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("ID:{0}.Count:{1}",this.ItemID,this.Count);
        }
    }
}
