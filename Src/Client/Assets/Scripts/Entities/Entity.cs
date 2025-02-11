using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SkillBridge.Message;

namespace Entities
{
    /// <summary>
    /// 实体对象游戏中能看见的东西实体对象
    /// </summary>
    public class Entity
    {
        public int entityId;


        public Vector3Int position;
        public Vector3Int direction;
        public int speed;


        /// <summary>
        /// 
        /// </summary>
        private NEntity entityData;//网络通讯的信息类，纯数据

        public NEntity EntityData
        {
            get {
                UpdateEntityData();//这个同步
                return entityData;
            }
            set {
                entityData = value;
                this.SetEntityData(value);//网络上对Nentity,同时更新entity,把网络的赋值给本地的
            }
        }

        public Entity(NEntity entity)
        {
            this.entityId = entity.Id;
            this.entityData = entity;
            this.SetEntityData(entity);
        }

        public virtual void OnUpdate(float delta)
        {
            if (this.speed != 0)
            {
                Vector3 dir = this.direction;
                this.position += Vector3Int.RoundToInt(dir * speed * delta / 100f);//移动
            }
        }

        /// <summary>
        /// 网络的数据转为本地的数据，
        /// </summary>
        /// <param name="entity"></param>
        public void SetEntityData(NEntity entity)
        {
            this.position = this.position.FromNVector3(entity.Position);
            this.direction = this.direction.FromNVector3(entity.Direction);
            this.speed = entity.Speed;
        }

        public void UpdateEntityData()
        {
            entityData.Position.FromVector3Int(this.position);
            entityData.Direction.FromVector3Int(this.direction);
            entityData.Speed = this.speed;
        }
    }
}
