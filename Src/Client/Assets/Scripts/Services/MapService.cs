
using Common.Data;
using Entities;
using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Services
{
    class MapService : Singleton<MapService>,IDisposable
    {
        public MapService()//响应事件注册，根据服务器返回来的消息
        {
            MessageDistributer.Instance.Subscribe<SkillBridge.Message.MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<SkillBridge.Message.MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Subscribe<SkillBridge.Message.MapEntitySyncResponse>(this.OnMapEntitySync);

        }

    
        public int CurrentMapId { get; set; }

        public void Dispose()//一般成对出现订阅和取消订阅
        {

            MessageDistributer.Instance.Unsubscribe<SkillBridge.Message.MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<SkillBridge.Message.MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Unsubscribe<SkillBridge.Message.MapEntitySyncResponse>(this.OnMapEntitySync);

        }

        public void Init()
        {

        }

        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse message)
        {
            Debug.LogFormat("OnMapCharacterEnter:Map {0} Count: {1}", message.mapId, message.Characters.Count);//协议服务端返回的message一个mapid 和一个角色列表

            foreach (var characters in message.Characters)
            {

                if ( User.Instance .CurrentCharacterInfo ==null|| (characters.Type==CharacterType.Player&& User.Instance.CurrentCharacterInfo.Id == characters.Id))
                {
                    //当前角色切换地图
                    User.Instance.CurrentCharacterInfo = characters;//为什么一样还要赋值，为了安全一下万一服务器数据发生改变，刷新一下数据
                    if (User.Instance.CurrentCharacter == null)///第一次进入游戏，没有实体才重新new一个
                    {
                        User.Instance.CurrentCharacter = new Entities.Character(characters);
                    }
                    else
                    {
                        User.Instance.CurrentCharacter.UpdateInfo(characters);
                    }

                    User.Instance.CharacterInited();

                    CharacterManager.Instance.AddCharacter(User.Instance.CurrentCharacter);//把服务器返回的所有角色交给角色管理器
                    Debug.LogFormat("OnMapCharacterEnter当前玩家信息名字：{0} ID：{1}", characters.Name, characters.Id);
                    continue;
                }
                CharacterManager.Instance.AddCharacter(new Character(characters));
            }

            if (CurrentMapId != message.mapId)//记录一下之前是不是这个地图，还是切换地图了
            {
                this.EnterMap(message.mapId);
                this.CurrentMapId = message.mapId;
                AudioManager.Instance.PlayMusic(DataManager.Instance.Maps[message.mapId].Music);
            }
        }

    

        private void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))//再判断一下地图id存不存在
            {
                MapDefine map = DataManager.Instance.Maps[mapId];
                User.Instance.CurrentMapDate = map;//切换地图把当前地图替换
                SceneManager.Instance.LoadScene(map.Resource);//加载地图
            }
            else
            {
                Debug.LogFormat("EnterMap:Map {0} not existed", mapId);
            }
        }

        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse message)
        {
            Debug.LogFormat("OnMapCharacterLeave: charID :{0}", message.entityId);

            if (message.entityId != User.Instance.CurrentCharacterInfo.EntityId)//如果是其他玩家离开，清除其他玩家
            {
                CharacterManager.Instance.RemoveCharacter(message.entityId);
            }
            else
                CharacterManager.Instance.Clear();//当前玩家全部清除

        }

        internal void SendMapEntitySyne(EntityEvent entityEvent, NEntity entityData,int param)
        {
           // Debug.LogFormat("OnMapEntityRequest: ID :{0}  POS:{1}  DIR: {2}  SPD：{3}", entityData.Id, entityData.Position.String(), entityData.Direction.String(), entityData.Speed); ;
            NetMessage netMessage = new NetMessage();
            netMessage.Request = new NetMessageRequest();
            netMessage.Request.mapEntitySync = new MapEntitySyncRequest();
            netMessage.Request.mapEntitySync.entitySync = new NEntitySync()//同步协议
            {
                Id = entityData.Id,
                Event = entityEvent,
                Entity = entityData,

                Param = param//这个参数后面加的坐骑的时候加的
                
            };
             NetClient.Instance.SendMessage(netMessage);
        }

        private void OnMapEntitySync(object sender, MapEntitySyncResponse response)
        {
            System.Text.StringBuilder sb = new StringBuilder();
            sb.AppendFormat("MapEntityUpdateResponse: Entity: {0}", response.entitySyncs.Count);
            sb.AppendLine();

            foreach (var entity in response.entitySyncs)
            {
                Managers.EntityManager.Instance.OnEntitySync(entity);
                sb.AppendFormat("     [{0}]evt: {1} entity: {2}", entity.Id, entity.Event, entity.Entity.String());
                sb.AppendLine();
            }
            Debug.Log(sb.ToString());
        }
        /// <summary>
        /// 传送发送消息
        /// </summary>
        /// <param name="telePorID"></param>
        internal void SendMapTeleport(int telePorID)
        {
            Debug.LogFormat("MapTeleporterRequest :teleporterID{0}", telePorID);
            NetMessage netMessage = new NetMessage();
            netMessage.Request = new NetMessageRequest();
            netMessage.Request.mapTeleport = new MapTeleportRequest();
            netMessage.Request.mapTeleport.teleporterId = telePorID;
            NetClient.Instance.SendMessage(netMessage);
        }
    }
}
