using Common;
using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    /// <summary>
    /// 对象管理器管理所有在线的玩家，进入游戏之后加入进来，离线删除
    /// </summary>
    class CharacterManager : Singleton<CharacterManager>
    {
        /// <summary>
        /// 存储所有的角色
        /// </summary>
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();
        //字典查询效率高，不需要遍历，遍历效率低

        public CharacterManager()
        {

        }
        public void Dispose()
        { }

        public void Init()
        { }

        public void Clear()
        {
            this.Characters.Clear();
        }

        public Character AddCharacter(TCharacter cha)
        {
            Character character = new Character(SkillBridge.Message.CharacterType.Player, cha);
            EntityManager.Instance.AddEntity(cha.MapID, character);
            character.Info.Id = character.Id;////同步从entity同步给网络，网络同步给entity
            this.Characters[character.Id ] = character;
            return character;
        }

        public void RemoveCharacter(int characterId)
        {
            var cha = this.Characters[characterId];
            EntityManager.Instance.RemoveEntity(cha.Data.MapID,cha);
            this.Characters.Remove(characterId);
        }
    }
}
