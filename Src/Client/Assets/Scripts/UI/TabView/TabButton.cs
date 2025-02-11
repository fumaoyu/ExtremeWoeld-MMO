
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class TabButton : MonoBehaviour
{
    public Sprite activeImage;
    private Sprite normalImage;

    public TabView tabView;


    public int tabIndex = -1;//选择的页
    public bool selected = false;//是否选择

    private Image tabImage;

    // Start is called before the first frame update
    void Start()
    {
        tabImage = this.GetComponent<Image>();
        normalImage = tabImage.sprite;

        // this.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void Select(bool select)
    {
        tabImage.overrideSprite = select ? activeImage : normalImage;
    }
    public void OnClick()
    {
        this.tabView.SelectTab(this.tabIndex);
        Debug.Log("点击了按钮{0}" );
        Log.InfoFormat("点击了按钮{0}", tabIndex);
    }
}

