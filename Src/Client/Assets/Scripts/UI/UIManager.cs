using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{

    /// <summary>
    /// UI元素类
    /// </summary>
    class UIElement
    {
        /// <summary>
        /// 资源路径
        /// </summary>
        public string Resources;
        /// <summary>
        /// 是否销毁还是隐藏
        /// </summary>
        public bool Cache;
        /// <summary>
        /// 要cache把instance存下来
        /// </summary>
        public GameObject Instance;
    }
    /// <summary>
    /// 保存ui信息
    /// </summary>
    private Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();

    public UIManager()
    {
        this.UIResources.Add(typeof(UITest), new UIElement() { Resources = "UI/UITest", Cache = true });//加入每一个ui界面
        this.UIResources.Add(typeof(UIBag), new UIElement() { Resources = "UI/UIBag", Cache = false });
        this.UIResources.Add(typeof(UIShop), new UIElement() { Resources = "UI/UIShop", Cache = false });
    }

    ~UIManager()
    {
    }
    // Update is called once per frame
    /// <summary>
    /// show UI
    /// </summary>
    /// <typeparam name="T">UI类型</typeparam>
    /// <returns></returns>
    public T Show<T>()
    {
       // SoundManager.Instance.PlaySound("ui_open");
        Type type = typeof(T);//类型
        if (this.UIResources.ContainsKey(type))
        {
            UIElement info = this.UIResources[type];
            if (info.Instance != null)//存在这个ui界面只是隐藏了
            {
                info.Instance.SetActive(true);
            }
            else
            {
                UnityEngine.Object prefab = Resources.Load(info.Resources);
                if (prefab == null)
                {
                    return default(T);
                }
                info.Instance =(GameObject) GameObject.Instantiate(prefab);

            }
            return info.Instance.GetComponent<T>();
        }
        return default(T);
    }

    public void Close(Type type)
    {
        // SoundManager.Instance.PlaySound("ui_close");

        if (this.UIResources.ContainsKey(type))
        {
            UIElement info = this.UIResources[type];
            if (info.Cache)//这个有没有启用，启用就不能销毁，而是隐藏
            {
                info.Instance.SetActive(false);//隐藏
            }

            else
            {
                GameObject.Destroy(info.Instance);//删除
                info.Instance = null;
            }
        }

    }
}
