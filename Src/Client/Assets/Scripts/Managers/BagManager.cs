
using Model;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using UnityEditor.PackageManager.Requests;

namespace Managers
{
    public class BagManager : Singleton<BagManager>
    {
        public int Unlocked;

        /// <summary>
        /// 背包格子里面的东西物品
        /// </summary>
        public BagItem[] Items;

        /// <summary>
        /// 网络传来的背包信息
        /// </summary>
        NBagInfo NBagInfo;

        unsafe public void Init(NBagInfo nBagInfo)
        {
            this.NBagInfo = nBagInfo;
            this.Unlocked = nBagInfo.Unlocked; ;
            Items = new BagItem[this.Unlocked];
            if (nBagInfo.Items != null && nBagInfo.Items.Length >= this.Unlocked)//大于之后重新申请内存
            {
                Analyze(nBagInfo.Items);
            }
            else
            {
                NBagInfo.Items = new byte[sizeof(BagItem) * this.Unlocked];
                Reset();
            }
        }

        /// <summary>
        /// 背包整理，背包数据就是玩家身上道具信息 
        /// </summary>
        public void Reset()
        {
            int i = 0;
            foreach (var item in ItemManager.Instance.Items)
            {
                if (item.Value.Count <= item.Value.Define.StackLimit)//是否超出最大限制
                {
                    this.Items[i].ItemId = (ushort)item.Key;
                    this.Items[i].Count = (ushort)item.Value.Count;
                }
                else
                {
                    int count = item.Value.Count;
                    while (count>item.Value.Define.StackLimit)//超出最大限制放入下一关格子
                    {
                        this.Items[i].ItemId = (ushort)item.Key;
                        this.Items[i].Count = (ushort)item.Value.Define.StackLimit;
                        i++;
                        count-=item.Value.Define.StackLimit; 

                    }
                    this.Items[i].ItemId = (ushort)item.Key;
                    this.Items[i].Count = (ushort)count;

                }
                i++;
            }
        }

        unsafe void Analyze(byte[] date)//字节变为bagitem数组
        {
            fixed (byte* pt = date)//fixed执行过程中这个地址不能改变
            {
                for (int i = 0; i < this.Unlocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));//将字节数据转为一个个小背包数据
                    Items[i] = *item;
                }
            }
        }

        unsafe public NBagInfo GetBagInfo()//bagitem变为字节数组
        {
            fixed (byte* pt = NBagInfo.Items)
            {
                for (int i = 0; i < this.Unlocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    *item = Items[i];
                }
            }
            return this.NBagInfo;
        }

        public void AddItem(int id, int count)
        {
            ushort addcount = (ushort)count;
            for (int i = 0; i < this.Items.Length; i++)
            {
                if (this.Items[i].ItemId == id)
                {
                    ushort canadd = (ushort)(DataManager.Instance.Items[id].StackLimit - this.Items[i].Count);
                    if (canadd > addcount)//增加的数量小于格子还可以增加的数量限制
                    {
                        this.Items[i].Count += addcount;
                        addcount = 0;
                        break;
                    }
                    else//大于放入下一关格子
                    {
                        this.Items[i].Count += canadd;
                        addcount -= canadd;
                    }
                }
            }
            if (addcount > 0)//放入后面空格//这里课程教的还不是很严谨，万一有很多东西大于两个格子最大限制，可以改造为循环语句
            {
                for (int i=0; i < Items.Length;i ++)
                {
                    if (this.Items[i].ItemId == 0)
                    {
                        this.Items[i].ItemId = (ushort)(id);
                        this.Items[i].Count = addcount;
                        break;
                    }
                }
            }
        }

        internal void RemoveItem(int id, int count)
        {
            //throw new NotImplementedException();
        }
    }
}
