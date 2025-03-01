﻿using Common;
using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class EntityManager:Singleton<EntityManager>
    {
        private int idx = 0;
        public List<Entity> Allentitys = new List<Entity>();
        public Dictionary<int, List<Entity>> MapEntities = new Dictionary<int, List<Entity>>();

        public void AddEntity(int mapId, Entity entity)
        {
            Allentitys.Add(entity);
            //加入管理器生成唯一id
            entity.EntityData.Id = ++this.idx;//加入一个角色数字加一，唯一id

            List<Entity> entities = null;

            if (!MapEntities.TryGetValue(mapId, out entities))//如果之前没有
            {
                entities = new List<Entity>();
                MapEntities[mapId] = entities;
            }
            entities.Add(entity);
        }
        public void RemoveEntity(int mapid, Entity entity)
        {
            this.Allentitys.Remove(entity);
            this.MapEntities[mapid].Remove(entity);
        }
    }
}
