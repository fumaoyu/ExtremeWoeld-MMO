using Common.Data;
using Entities;
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
            CurrentCharacterInfo.Diamond += v;
        }

        internal void AddGold(int v)
        {
            CurrentCharacterInfo.Gold += v;
        }

        public Character CurrentCharacter { get; set; }

        public SkillBridge.Message.NCharacterInfo CurrentCharacterInfo { get; set; }

        public NTeamInfo TeamInfo { get; set; }//队伍信息这样添加数据用着方便

        public MapDefine CurrentMapDate { get; set; }

        public PlayerInputController CurrentCharacterObject;

        public int CurrentRide = 0;//坐骑id

        /// <summary>
        /// 设置坐骑id
        /// </summary>
        /// <param name="Id"></param>
        public void Ride(int Id)
        {
            if (CurrentRide != Id )
            {
                CurrentRide=Id;
                CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride, CurrentRide);
            }
            ///想换坐骑只能先下，再上，后面可以再改
            else
            {
                CurrentRide = 0;
                CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride, 0);
            }

        }

        public delegate void CharacterInitHandle();
        public event CharacterInitHandle OnCharacter;

        public void CharacterInited()
        {
            if (OnCharacter != null)
            {
                OnCharacter();
            }
        }
    }
}
