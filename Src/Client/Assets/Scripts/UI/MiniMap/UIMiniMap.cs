using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class UIMiniMap : MonoBehaviour
{
    public Button maxBtn;
    public Button minBtn;

    public Image miniMap;
    public Text MiniName;

    public Image arrow;

    public Collider miniMapBox;

    public Transform playertransform;
    // Start is called before the first frame update
    void Start()
    {
        MiniMapManager.Instance.miniMap = this;
        this.UpdateMap();
    }

    public void UpdateMap()
    {
        this.MiniName.text = User.Instance.CurrentMapDate.Name;
        //User.Instance.CurrentMapDate.Resource
        //if(this.miniMap.sprite != MiniMapManager.Instance.LoadCurrentMiniMap())
        this.miniMap.overrideSprite = MiniMapManager.Instance.LoadCurrentMiniMap();
        this.miniMap.SetNativeSize();
        this.miniMap.transform.localPosition = Vector3.zero;

        this.miniMapBox = MiniMapManager.Instance.miniMapBoundingBox;
        this.playertransform = null;
    }
    // Update is called once per frame
    void Update()
    {
        if (playertransform == null)
            playertransform = MiniMapManager.Instance.PLayerTransform;
        if (miniMapBox == null || playertransform == null)
            return;
        float realWidth = this.miniMapBox.bounds.size.x;
        float realHeight = this.miniMapBox.bounds.size.z;
        ///角色相对包围盒位置  用世界位置计算 
        float relativeX = playertransform.position.x - this.miniMapBox.bounds.min.x;
        float relativeY = playertransform.position.z - this.miniMapBox.bounds.min.z;

        //中心位置移动
        float pivotX = relativeX / realWidth;
        float pivotY = relativeY / realHeight;

        this.miniMap.rectTransform.pivot = new Vector2(pivotX,pivotY);
        this.miniMap.rectTransform.localPosition = Vector2.zero;
        //小地图指示旋转，角色绕y小指针绕z轴
        this.arrow.transform.eulerAngles=new Vector3(0,0,-playertransform.eulerAngles.y);
    }
}
