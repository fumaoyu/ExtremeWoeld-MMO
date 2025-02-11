using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Managers
{
    interface IEntityNotify//接口加事件，接口实现的事件
    {
        void OnEntityRemoved();
        void OnEntityChanged(Entity entity);
        void OnEntityEvent(EntityEvent @event);
    }
    class EntityManager : Singleton<EntityManager>
    {

        Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
        Dictionary<int, IEntityNotify> notifiers = new Dictionary<int, IEntityNotify>();

        public void RegisterEntityChangeNotify(int entityId, IEntityNotify notify)
        {
            this.notifiers[entityId] = notify;
        }

        public void AddEntity(Entity entity)
        {
            entities[entity.entityId] = entity;
        }

        public void RemoveEntity(NEntity entity)
        {
            this.entities.Remove(entity.Id);
            if (notifiers.ContainsKey(entity.Id))
            {
                notifiers[entity.Id].OnEntityRemoved();
               
                notifiers.Remove(entity.Id);
            }
        }

        /// <summary>
        /// 有其他人变了要同步，在自己这边找到别人的entity，然后改变
        
        /// </summary>
        /// <param name="data"></param>
        internal void OnEntitySync(NEntitySync data)
        {
            Entity entity = null;
            entities.TryGetValue(data.Id, out entity);//根据服务器传来的data找要同步的
            if (entity != null)//有没有找到
            {
                if (data.Entity != null)
                {
                    entity.EntityData = data.Entity;//找到了赋值
                }
                if (notifiers.ContainsKey(data.Id))
                {
                    ///两个通知 ，自己的一个事件通知
                    ///
                    notifiers[entity.entityId].OnEntityChanged(entity);//entity数据变了
                    notifiers[entity.entityId].OnEntityEvent(data.Event);//entity状态发生改变了
                }
            }
        }
    }
}
