using Managers;
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
        this.UIResources.Add(typeof(UIBag), new UIElement() { Resources = "UI/UIBag", Cache = false });//这里改为false，为了关闭之后打开重新创建执行start刷新界面，如果为true，只是隐藏再次打开不会执行start，方便刷新，后面可以优化
        this.UIResources.Add(typeof(UIShop), new UIElement() { Resources = "UI/UIShop", Cache = false });
        this.UIResources.Add(typeof(UICharEquip), new UIElement() { Resources = "UI/UICharEquip", Cache = false });
        this.UIResources.Add(typeof(UIQuestSystem), new UIElement() { Resources = "UI/UIQuestSystem", Cache = false });//任务系统界面
        this.UIResources.Add(typeof(UIQuestDialog), new UIElement() { Resources = "UI/UIQuestDialog", Cache = false });//任务对话界面
        this.UIResources.Add(typeof(UIFriend), new UIElement() { Resources = "UI/UIFriends", Cache = false });//任务对话界面
        this.UIResources.Add(typeof(UIGuild), new UIElement() { Resources = "UI/Guild/UIGuild", Cache = false });//自己的公会界面
        this.UIResources.Add(typeof(UIGuildList), new UIElement() { Resources = "UI/Guild/UIGuildList", Cache = false });//公会列表界面
        this.UIResources.Add(typeof(UIGuildPopNoGuild), new UIElement() { Resources = "UI/Guild/UIGuildPopNoGuild", Cache = false });//没有公会界面
        this.UIResources.Add(typeof(UIGuildPopCreate), new UIElement() { Resources = "UI/Guild/UIGuildPopCreate", Cache = false });//创建界面
        this.UIResources.Add(typeof(UIGuildApplyList), new UIElement() { Resources = "UI/Guild/UIGuildPopApplyList", Cache = false });//创建界面
        this.UIResources.Add(typeof(UISetting), new UIElement() { Resources = "UI/UISetting", Cache = false });//设置界面
        this.UIResources.Add(typeof(UIPopChatMenu), new UIElement() { Resources = "UI/UIPopChatMenu", Cache = false });//聊天界面
        this.UIResources.Add(typeof(UIChat), new UIElement() { Resources = "UI/UIChat", Cache = false });//聊天界面
        this.UIResources.Add(typeof(UIRide), new UIElement() { Resources = "UI/UIRide", Cache = false });//坐骑
        this.UIResources.Add(typeof(UISystemConfig), new UIElement() { Resources = "UI/UISystemConfig", Cache = false });//音乐设置界面
        this.UIResources.Add(typeof(UISkill), new UIElement() { Resources = "UI/UISkill", Cache = false });//技能
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
       AudioManager.Instance.PlaySound(SoundDefine.SFX_UI_Open);

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

    public  void Close<T>()
    {
        Close(typeof(T));
        //int a = 0;
        //Debug.LogWarning(typeof(int));
         
    }

    public void Close(Type type)
    {
         AudioManager.Instance.PlaySound(SoundDefine.SFX_UI_Close);

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
