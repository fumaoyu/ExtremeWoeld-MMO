using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    class MiniMapManager : Singleton<MiniMapManager>
    {
        /// <summary>
        /// 每个地图的包围盒
        /// </summary>
        public Collider miniMapBoundingBox;

        /// <summary>
        /// 小地图
        /// </summary>
        public UIMiniMap miniMap;

        public Collider MiniMapBoundingBox
        {
            get { return miniMapBoundingBox; }
        }

        public Transform PLayerTransform
        {
            get
            {
                if (User.Instance.CurrentCharacterObject == null)
                {
                    return null;
                }
                return User.Instance.CurrentCharacterObject.transform;
            }
        }

        public Sprite LoadCurrentMiniMap()
        {
            return Resloader.Load<Sprite>("UI/Minimap/" + User.Instance.CurrentMapDate.MiniMap);
        }

        public void UpdateMiniMap(Collider miniBoundingBox)
        {
            this.miniMapBoundingBox = miniBoundingBox;
            if (this.miniMap != null)//包围盒变了说明地图更新了
            {
                this.miniMap.UpdateMap();
            }

        }
    }

}
