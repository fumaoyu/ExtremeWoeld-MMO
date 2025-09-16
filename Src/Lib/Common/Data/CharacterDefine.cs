using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;

namespace Common.Data
{
    public class CharacterDefine
    {
        public int TID { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 职业
        /// </summary>
        public CharacterClass Class { get; set; }
        public string Resource { get; set; }
       /// <summary>
       /// 描述
       /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 身高
        /// </summary>
        public float Height { get; set; }

        //基本属性
        public int Speed { get; set; }
        public float MaxHP { get; set; }
        public float MaxMP { get; set; }

        //成长属性
        public float GrowthSTR { get; set; }
        public float GrowthINT { get; set; }
        public float GrowthDEX { get; set; }

        /// <summary>
        /// 力量
        /// </summary>
        public float STR { get; set; }
        /// <summary>
        /// 智力
        /// </summary>
        public float INT { get; set; }
        /// <summary>
        /// 敏捷
        /// </summary>
        public float DEX { get; set; }

        /// <summary>
        /// 物理攻击
        /// </summary>
        public float AD { get; set; }
        /// <summary>
        /// 法术攻击
        /// </summary>
        public float AP { get; set; }
        /// <summary>
        /// 物理防御
        /// </summary>
        public float DEF { get; set; }
        /// <summary>
        /// 法术防御
        /// </summary>
        public float MDEF { get; set; }
        /// <summary>
        /// 攻击速度
        /// </summary>
        public float SPD { get; set; }
        /// <summary>
        /// 暴击概率
        /// </summary>
        public float CRI { get; set; }



    }
}
