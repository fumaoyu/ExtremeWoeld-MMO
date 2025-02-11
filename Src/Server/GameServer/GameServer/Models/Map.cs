using Common;
using Common.Data;
using GameServer.Entities;
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

        internal MapDefine Define;

        /// <summary>
        /// 
        /// </summary>
        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();

        internal Map(MapDefine define)
        {
            this.Define = define;
        }

        internal void Update() { }

        /// <summary>
        /// 角色进入地图
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="character"></param>
        internal void CharacterEnter(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("CharacterEnter: Map: {0} character: {1}", this.Define.ID, character.Id);

            character.Info.mapId = this.ID;

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();

            message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            message.Response.mapCharacterEnter.mapId = this.Define.ID;
            message.Response.mapCharacterEnter.Characters.Add(character.Info);

            foreach (var item in this.MapCharacters)//把当前这个玩家进入地图的信息发送给当前所有的玩家
            {
                message.Response.mapCharacterEnter.Characters.Add(item.Value.character.Info);
                this.SendCharacterEnterMap(item.Value.connection, character.Info);
            }

            this.MapCharacters[character.Id] = new MapCharacter(conn, character);

            byte[] date = PackageHandler.PackMessage(message);
            conn.SendData(date, 0, date.Length);//然后再告诉自己\这个conn代指的自己

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
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();

            message.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            message.Response.mapCharacterLeave.characterId = this.Define.ID;
            //message.Response.mapCharacterLeave.(character);

            byte[] date = PackageHandler.PackMessage(message);
            connection.SendData(date, 0, date.Length);
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
    }
}
