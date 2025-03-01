using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Models
{
    class User : Singleton<User>
    {
        SkillBridge.Message.NUserInfo userInfo;


        public SkillBridge.Message.NUserInfo Info
        {
            get { return userInfo; }
        }


        public void SetupUserInfo(SkillBridge.Message.NUserInfo info)
        {
            this.userInfo = info;
        }

        internal void AddDiamond(int v)
        {
            CurrentCharacter.Diamond += v;
        }

        internal void AddGold(int v)
        {
            CurrentCharacter.Gold += v;
        }

        public SkillBridge.Message.NCharacterInfo CurrentCharacter { get; set; }

        public NTeamInfo TeamInfo { get; set; }//队伍信息这样添加数据用着方便

        public MapDefine CurrentMapDate { get; set; }

        public GameObject CurrentCharacterObject;
    }
}
