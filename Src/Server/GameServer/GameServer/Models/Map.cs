using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Map
    {
        internal class MapCharacter
        {
            public NetConnection<NetSession> connection;
            public Character character;

            /// <summary>
            /// 地图角色
            /// </summary>
            /// <param name="conn">  </param>
            /// <param name="character">  </param>
            public MapCharacter(NetConnection<NetSession> conn, Character character)
            {
                this.connection = conn;
                this.character = character;
            }
        }

        public int ID
        {
            get { return this.Define.ID; }
        }

        /// <summary>
        /// 当前哪个地图
        /// </summary>
        internal MapDefine Define;

        /// <summary>
        /// 地图中的角色以characterid为key
        /// </summary>
        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();

        /// <summary>
        /// 刷怪管理器
        /// </summary>
        SpawnManager spawnManager = new SpawnManager();

        public MonsterManager monsterManager = new MonsterManager();
        /// 

        internal Map(MapDefine define)
        {
            this.Define = define;
            this.spawnManager.Init(this);
            this.monsterManager.Init(this);//两个管理器初始化，把当前哪个地图传进去
        }

        internal void Update()
        {
            spawnManager.Update();////刷怪更新
        }

        /// <summary>
        /// 角色进入地图
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="character"></param>
        internal void CharacterEnter(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("CharacterEnter: Map: {0} character: {1}", this.Define.ID, character.Id);

            character.Info.mapId = this.ID;

            //NetMessage message = new NetMessage();
            //message.Response = new NetMessageResponse();

            //message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            //message.Response.mapCharacterEnter.mapId = this.Define.ID;
            //message.Response.mapCharacterEnter.Characters.Add(character.Info);

            //foreach (var item in this.MapCharacters)//把当前这个玩家进入地图的信息发送给当前   地图所有的玩家
            //{
            //    message.Response.mapCharacterEnter.Characters.Add(item.Value.character.Info);//把当前地图的其他玩家加入到新进入玩家的信息里面
            //    this.SendCharacterEnterMap(item.Value.connection, character.Info);//把新加入的玩家信息告诉地图上的其他玩家
            //}

            //this.MapCharacters[character.Id] = new MapCharacter(conn, character);//加入到字典中

            //byte[] date = PackageHandler.PackMessage(message);
            //conn.SendData(date, 0, date.Length);//然后再告诉自己\这个conn代指的自己


            ///---------------------------------------------优化后
            this.MapCharacters[character.Id] = new MapCharacter(conn, character);
            conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            conn.Session.Response.mapCharacterEnter.mapId = this.Define.ID;
            ////玩家进入地图
            foreach (var cha in this.MapCharacters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(cha.Value.character.Info);//
                if (cha.Value.character != character)
                {
                    this.AddCharacterEnterMap(cha.Value.connection, character.Info);
                }
            }
            ///怪物进入地图
            foreach (var monster in this.monsterManager.Monsters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(monster.Value.Info);
            }
            conn.SendResponse();///因为对底层进行重构过才可以这样使用直接用
        }


        /// <summary>
        /// 发送给地图所有人我要进入地图了
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="character"></param>
        private void SendCharacterEnterMap(NetConnection<NetSession> connection, NCharacterInfo character)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();

            message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            message.Response.mapCharacterEnter.mapId = this.Define.ID;
            message.Response.mapCharacterEnter.Characters.Add(character);

            byte[] date = PackageHandler.PackMessage(message);
            connection.SendData(date, 0, date.Length);
        }

        internal void CharacterLeave(Character cha)
        {
            
            Log.InfoFormat("CharacterLeave: Map: {0} character: {1}", this.Define.ID, cha.Id);
         

            foreach (var item in MapCharacters)//告诉通知地图上的所有其他在线人我离开地图了，这样其他玩家才能知道
            {
                this.SendCharacterLeaveMap(item.Value.connection, cha);
            }
            this.MapCharacters.Remove(cha.Id);
        }

        private void SendCharacterLeaveMap(NetConnection<NetSession> connection, Character character)
        {
            //NetMessage message = new NetMessage();
            //message.Response = new NetMessageResponse();

            //message.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            //message.Response.mapCharacterLeave.characterId = this.Define.ID;
            ////message.Response.mapCharacterLeave.(character);

            //byte[] date = PackageHandler.PackMessage(message);
            //connection.SendData(date, 0, date.Length);

            connection.Session.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            connection.Session.Response.mapCharacterLeave.entityId = character.entityId;
            connection.SendResponse();
        }
        /// <summary>
        /// 同步更新信息广播给当前地图的所有玩家
        /// </summary>
        /// <param name="entitySync"></param>
        internal void UpdateEntity(NEntitySync entitySync)
        {
            foreach (var item in this.MapCharacters)
            {
                if (item.Value.character.entityId == entitySync.Id)//是自己t更新服务器entity信息
                {
                    item.Value.character.Position = entitySync.Entity.Position;
                    item.Value.character.Direction = entitySync.Entity.Direction;
                    item.Value.character.Speed = entitySync.Entity.Speed;
                }
                else///其他玩家发送我的entity同步信息
                {
                    MapService.Instance.SendEntityUpdate(item.Value.connection, entitySync);
                }
            }
        }

        public void MonsterEnter(Monster monster)
        {
            Log.InfoFormat("MonsterEnter: Map{0}  monster:{1}", this.Define.ID, monster.Id);

            foreach (var character in  this.MapCharacters)///遍历地图是所有玩家，告诉TM有怪物加进来了
            {
                this.AddCharacterEnterMap(character.Value.connection, monster.Info);
            }
        }

        /// <summary>
        /// 和send不一样，
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="info"></param>
        private void AddCharacterEnterMap(NetConnection<NetSession> connection, NCharacterInfo character)
        {
            //先判断角色是不是
            if (connection.Session.Response.mapCharacterEnter == null)
            {
                connection.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
                connection.Session.Response.mapCharacterEnter.mapId = this.Define.ID;
            }
            connection.Session.Response.mapCharacterEnter.Characters.Add(character);
            connection.SendResponse();//后面会改 
        }
    }
}
