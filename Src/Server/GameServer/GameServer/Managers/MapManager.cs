using Common;
using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    /// <summary>
    /// 地图管理器单例类
    /// </summary>
    class MapManager : Singleton<MapManager>
    {
        Dictionary<int, Map> Maps = new Dictionary<int, Map>();

        public void Init()
        {
            foreach (var mapdefine in DataManager.Instance.Maps.Values)
            {
                Map map = new Map(mapdefine);
                Log.InfoFormat("MapManager.Init > Map:{0}:{1}", map.Define.ID, map.Define.Name);
                this.Maps[mapdefine.ID] = map;
            }
        }


        /// <summary>
        /// 地图索引器方便访问Maps字典管理key得到value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Map this[int key]
        {
            get
            {
                return this.Maps[key];
            }
        }

        /// <summary>
        /// 更新地图里面内容怪物等  自动
        /// </summary>
        public void Update()
        {
            foreach (var map in Maps.Values)
            {
                map.Update();
            }
        }
    }
}
