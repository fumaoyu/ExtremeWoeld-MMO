using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Model
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]//属性，这是一种结构布局
    public   struct BagItem//用结构体为了将来两个背包格子做交换，值类型交换，类是引用类型不方便
    {
        public ushort ItemId;//id
        public ushort Count;//数量

        public static BagItem zero = new BagItem { ItemId = 0, Count = 0 };

        public BagItem(int itemid, int count)
        {
            this.ItemId = (ushort)itemid;
            this.Count = (ushort)count;
        }

        //重载一下运算符为了和其他格子比较方便一点
        public static bool operator ==(BagItem a, BagItem b)
        {
            return a.ItemId == b.ItemId && a.Count == b.Count;
        }

        public static bool operator !=(BagItem a, BagItem b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is BagItem)
            {
                return Equals((BagItem)obj);
            }
            return false;
        }

        public bool Equals(BagItem other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return ItemId.GetHashCode() ^ (Count.GetHashCode()<<2);
        }
    }


}
