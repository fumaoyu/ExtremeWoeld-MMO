using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;
using UnityEngine.Events;
using Entities;
using SkillBridge.Message;
using Models;

namespace Managers
{
    class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

        /// <summary>
        /// j角色进入地图事件
        /// </summary>
        public UnityAction<Character> OnCharacterEnter;
        /// <summary>
        /// 角色离开事件
        /// </summary>
        public UnityAction<Character> OnCharacterLeave;

        public CharacterManager()
        {
         
        }

        
        public void Dispose()
        {
        }

        public void Init()
        {

        }

        public void Clear()
        {
            int[] keys = this.Characters.Keys.ToArray();
            foreach (var key in keys)
            {
                this.RemoveCharacter(key);
            }
            this.Characters.Clear();
        }

        public void AddCharacter(Character cha)
        {
            Debug.LogFormat("AddCharacter:{0}:{1} Map:{2} Entity:{3}", cha.Id, cha.Name, cha.Info.mapId, cha.Info.Entity.String());
            //Character character = new Character(cha);////重构
            this.Characters[cha.entityId] = cha;
            EntityManager.Instance.AddEntity(cha);//添加到管理器中
            if (OnCharacterEnter != null)
            {
                OnCharacterEnter(cha);
            }




            //if (cha.EntityId == User.Instance.CurrentCharacterInfo.EntityId)///是不是当前角色
            //{
            //    User.Instance.CurrentCharacter = character;
            //}
        }


        public void RemoveCharacter(int entityId)
        {
            Debug.LogFormat("RemoveCharacter:{0}", entityId);
            if (this.Characters.ContainsKey(entityId))
            {
                EntityManager.Instance.RemoveEntity(this.Characters[entityId].Info.Entity);
                if (OnCharacterLeave != null)
                {
                    OnCharacterLeave(this.Characters[entityId]);
                }
                this.Characters.Remove(entityId);
            }
        }

        public Character GetCharacter(int id)
        {
            Character character;
            this.Characters.TryGetValue(id, out character);
            return character;
        }
    }
}
