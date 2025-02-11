using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;

namespace Common.Data
{
    /// <summary>
    /// npc种类
    /// </summary>
    public enum NpcType
    {
        None=0,
        Functional=1,
        Task,
    }

    /// <summary>
    /// npc功能
    /// </summary>
    public enum NpcFunction
    {
        None=0,
        InvokeShop= 1,
        InvokeInsrance=2,
    }
    public class NpcDefine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Descrip { get; set;}
        public NVector3 Position { get; set; }
        public NpcType Type { get; set; }
        public NpcFunction Function { get; set; }
        public int Param { get; set; }
       
    }
}
