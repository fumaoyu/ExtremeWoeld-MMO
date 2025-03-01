using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Managers
{
    class NpcManager:Singleton<NpcManager>
    {
        public delegate bool NpcActionHandler(NpcDefine npc);

        /// <summary>
        /// 字典，一个function对应一个委托，表里面配置了funtion
        /// </summary>
        Dictionary<NpcFunction, NpcActionHandler> eventMap = new Dictionary<NpcFunction, NpcActionHandler>();

        /// <summary>
        ///根据npc功能添加事件委托..注册
        /// </summary>
        /// <param name="function"></param>
        /// <param name="action"></param>
        public void RegisterNpcEvent(NpcFunction function, NpcActionHandler action)
        {
            if (!eventMap.ContainsKey(function))
            {
                eventMap[function] = action;
            }
            else
                eventMap[function] += action;
        }

        public NpcDefine GetNpcDefine(int id)
        {
            NpcDefine npc = null;
            DataManager.Instance.Npcs.TryGetValue(id, out npc);//这样写为了安全
            return npc;
         // return DataManager.Instance.Npcs[id];
        }

        /// <summary>
        /// 交互
        /// </summary>
        /// <param name="npcid"></param>
        /// <returns></returns>
        public bool Interactive(int npcid)
        {
            if (DataManager.Instance.Npcs.ContainsKey(npcid))//先判断npc存不存在
            {
                var npc = DataManager.Instance.Npcs[npcid];//存在执行
                return Interactive(npc);
            }
            return false;
        }

        public  bool Interactive(NpcDefine npc)
        {
            if (DoTaskInteractive(npc))//任务型npc、、、、、、、、、、、、
            {
                return true;
            }
            else if (npc.Type == NpcType.Functional)//功能型npc
            {
                return DoFunctionInteractive(npc);
            }
            return false;
        }

        /// <summary>
        /// 任务npc
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        private bool DoTaskInteractive(NpcDefine npc)
        {
            var status = QuestManager.Instance.GetQuestStatusByNpc(npc.ID);
            if (status == NpcQuestStatus.None)//没有任务
            {
                return false;
            }
            return QuestManager.Instance.OpenNpcQuest(npc.ID);

            //MessageBox.Show("点击了NPC " + npc.Name, "NPC对话");
            //return true;
        }

        /// <summary>
        /// 功能npc交互
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        private bool DoFunctionInteractive(NpcDefine npc)
        {
            if (npc.Type != NpcType.Functional)
            {
                return false;
            }
            if (!eventMap.ContainsKey(npc.Function))
                return false;
            return eventMap[npc.Function](npc);
        }

  
    }
}
