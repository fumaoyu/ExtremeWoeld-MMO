using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class MapService : Singleton<MapService>
    {
        public MapService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnMapEntitySync);
            ////传送点注册
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapTeleportRequest>(this.OnMapTeleport);
        }



        public void Init()
        {
            MapManager.Instance.Init();
        }
        /// <summary>
        /// 同步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest request)
        {
            Character character = sender.Session.Character;//谁移动同步
            Log.InfoFormat("OnEntitySync: characterID:{0}:{1} Entity.Id:{2} Evt:{3} Entity:{4}", character.Id, character.Info.Name, request.entitySync.Id, request.entitySync.Event, request.entitySync.Entity.String());
            MapManager.Instance[character.Info.mapId].UpdateEntity(request.entitySync);//找到地图
        }

        //internal void SendEntityUpdate(NetConnection<NetSession> connection, NEntitySync entitySync)
        //{
        //    throw new NotImplementedException();
        //}

        internal void SendEntityUpdate(NetConnection<NetSession> conn, NEntitySync entity)
        {
            //NetMessage netMessage = new NetMessage();
            //netMessage.Response = new NetMessageResponse();

            //netMessage.Response.mapEntitySync = new MapEntitySyncResponse();
            //netMessage.Response.mapEntitySync.entitySyncs.Add(entity);
            //byte[] data = PackageHandler.PackMessage(netMessage);
            //conn.SendData(data, 0, data.Length);

            conn.Session.Response.mapEntitySync = new MapEntitySyncResponse();
            conn.Session.Response.mapEntitySync.entitySyncs.Add(entity);

            conn.SendResponse();

        }

        private void OnMapTeleport(NetConnection<NetSession> sender, MapTeleportRequest request)
        {
            Character character = sender.Session.Character;//是谁传送

            Log.InfoFormat("OnMapTeleport: characterID:{0}:{1} TeleporterID:{2}", character.Id, character.Data, request.teleporterId);
            if (!DataManager.Instance.Teleporters.ContainsKey(request.teleporterId))//看传送点对不对
            {
                Log.WarningFormat("Source TeleportID [{0}] not existed", request.teleporterId);
                return;
            }
            TeleporterDefine source = DataManager.Instance.Teleporters[request.teleporterId];//从配置表中读出传送点数据
            if (source.LinkTo == 0 ||! DataManager.Instance.Teleporters.ContainsKey(source.LinkTo))//判断表里面的连接点对不对
            {
                Log.WarningFormat("Source TeleportID [{0}] LinkTo ID [{1}] not existed", request.teleporterId, source.LinkTo);
            }
            TeleporterDefine target = DataManager.Instance.Teleporters[source.LinkTo];//得到传送目标

            MapManager.Instance[source.MapID].CharacterLeave(character);//角色离开原地图
                                                                        //设置进入角色新地图的坐标和方向传送点
            character.Position = target.Position;
            character.Direction = target.Direction;

            MapManager.Instance[target.MapID].CharacterEnter(sender, character);//进入新地图

            // }
        }

    }
}
    
