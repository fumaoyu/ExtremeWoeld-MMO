using Common;
using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using GameServer.Models;
using GameServer.Network;
using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    class Character : CharacterBase,IPostResponser
    {
       
        public TCharacter Data;//数据库数据
        //每个角色数据不一样。要加角色身上，manager
        public ItemManager ItemManager;//每个角色不一样，就需要一个manager,像地图manager,为所有玩家共有的，角色身上就不需要带
        public StatusManager StatusManager;
        public QuestManager QuestManager;
        public FriendManager FriendManager;
        /// <summary>
        /// /组队没有db
        /// </summary>
        public Team Team;
        public double TeamUpdateTS;//时间戳

        public Guild Guild;//公会
        public double GuildUpdateTs;//公会更新时间戳

        public Character(CharacterType type,TCharacter cha)://从db数据赋值到网络
            base(new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ),new Core.Vector3Int(100,0,0))
        {
            this.Data = cha;
            this.Id = cha.ID;
            this.Info = new NCharacterInfo();//db数据赋值给网络
            this.Info.Type = type;
            this.Info.Id = cha.ID;
            this.Info.EntityId = this.entityId;
            this.Info.Name = cha.Name;
            this.Info.Level = 15;//cha.Level;
            this.Info.ConfigId = cha.TID;
            this.Info.Class = (CharacterClass)cha.Class;
            this.Info.mapId = cha.MapID;
            this.Info.Gold = cha.Gold;
            this.Info.Diamond = cha.Diamond;
            this.Info.Entity = this.EntityData;
            this.Define = DataManager.Instance.Characters[this.Info.ConfigId];

            this.ItemManager = new ItemManager(this);
            this.ItemManager.GetItemInfos(this.Info.Items);

            this.Info.Bag = new NBagInfo();
            this.Info.Bag.Unlocked = this.Data.CharacterBag.Unlocked;
            this.Info.Bag.Items = this.Data.CharacterBag.Items;
            this.Info.Equips = this.Data.Equips;
            this.StatusManager = new StatusManager(this);
            this.QuestManager = new QuestManager(this);
            this.QuestManager.GetQuestInfos(this.Info.Quests);//数据库数据赋值给网络
            this.FriendManager = new FriendManager(this);
            this.FriendManager.GetFriendInfos(this.Info.Friends);
            this.Guild = GuildManager.Instance.GetGuild(this.Data.GuildID);
        }

        public NCharacterInfo GetBasicInfo(NCharacterInfo info)
        {
            return new NCharacterInfo()
            {
                Id = info.Id,
                Name = info.Name,
                Class = info.Class,
                Level = info.Level
            };
        }

        public long Gold
        {
            get { return this.Data.Gold; }
            set
            {
                if (this.Data.Gold == value)
                    return;
                this.StatusManager.AddGoldChange((int)(value - this.Data.Gold));
                this.Data.Gold = value;
            }
        }
        public int  Diamond
        {
            get { return this.Data.Diamond; }
            set
            {
                if (this.Data.Diamond == value)
                    return;
                this.StatusManager.AddDiamondChange(value - this.Data.Diamond);//金币的变化，增加不关心
                this.Data.Diamond = value;
            }
        }

        /// <summary>
        /// 后处理
        /// </summary>
        /// <param name="message"></param>
       public void PostProcess(NetMessageResponse message)
        {
            Log.InfoFormat("PostProcess-> CharacteID:{0} Name:{1}", this.Id, this.Info.Name);

            this.FriendManager.PostProcess(message);///好友管理器后处理

            if (this.Team != null)/////组队信息
            {
                Log.InfoFormat("PostProcess->Team：CharacterID:{0} Name:{1}", this.Id, this.Info.Name);
                
                if (TeamUpdateTS < this.Team.timestamp)///时间戳，TeamUpdateTS每个人身上的队伍信息变更时间，timestamp，队伍信息变更时间
                {
                    Log.InfoFormat("TeamUpdateTS:{0} timestamp当前更新时间：{1} ", TeamUpdateTS, this.Team.timestamp);
                    TeamUpdateTS = Team.timestamp;
                    this.Team.PostProcess(message);
                }
            }

            if (this.Guild != null)
            {
                Log.InfoFormat("PostProcess->Guild：CharacterID:{0} Name:{1}   {2}<{3}", this.Id, this.Info.Name,GuildUpdateTs,this.Guild.timeStamp);
                if (this.Info.Guild == null)
                    {
                    this.Info.Guild = this.Guild.GuildInfo(this);
                    if (message.mapCharacterEnter != null)///不是第一次登录设置一下
                        GuildUpdateTs = Guild.timeStamp;
                    }
                if (GuildUpdateTs < this.Guild.timeStamp && message.mapCharacterEnter == null)
                {
                    GuildUpdateTs = Guild.timeStamp;
                    this.Guild.PostProcess(this, message);
                }
            }


            if (this.StatusManager.HasStatus)
            {
                this.StatusManager.PostProcess(message);///状态管理器后处理
            }
        }

        /// <summary>
        /// 角色离开调用
        /// </summary>
        public void Claer()
        {
            //this.FriendManager.UpdateFriendInfo(this.Info, 0);
            this.FriendManager.OfflineNotify();
        }

        public NCharacterInfo GetBasicInfo()
        {
            return new NCharacterInfo()
            {
                Id = this.Id,
                Name = this.Info.Name,
                Class = this.Info.Class,
                Level = this.Info.Level
            };
        }

        //void IPostResponser.PostProcess(NetMessageResponse message)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
