using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    class TaskManager:Singleton<TaskManager>
    {
        public void Init()
        {
           
            NpcManager.Instance.RegisterNpcEvent(Common.Data.NpcFunction.InvokeShop, OnNpcInvokeShop);

            NpcManager.Instance.RegisterNpcEvent(NpcFunction.InvokeInsrance, OnNpcInvokeInsrance);
        }

        public bool OnNpcInvokeShop(NpcDefine npcdefine)
        {
            Debug.LogFormat("TeskaMansger.OnNpcInvokeShop :NPC :[{0}:{1}], Type: {2} ,Function : {2}", npcdefine.ID, npcdefine.Name, npcdefine.Type, npcdefine.Function);
           // UITest ui = UIManager.Instance.Show<UITest>();
            //ui.UpdateText(npcdefine.Name);
            return true;
        }
        public bool OnNpcInvokeInsrance(NpcDefine npcDefine)
        {
            //MessageBox.Show("点击了NPC"+ npcDefine.Name, "NPC对话");
            return true;
        }

    }
}
