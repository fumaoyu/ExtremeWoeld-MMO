using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     所有UI的父类
/// </summary>
public class UIWindow : MonoBehaviour
{
   /// <summary>
   /// 关闭的委托
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="result"></param>
    public delegate void CloseHandler(UIWindow sender, WindowResult result);
    /// <summary>
    ///内置的一个 关闭事件
    /// </summary>
    public event CloseHandler OnClose;

    public virtual Type type { get { return this.GetType(); } }

    /// <summary>
    /// 结果类型
    /// </summary>
    public enum WindowResult
    {
        None=0,
        Yes,
        Np,
    }


    public void Close(WindowResult result=WindowResult.None)
    {
        UIManager.Instance.Close(this.type);
        if (this.OnClose != null)
            this.OnClose(this, result);
        this.OnClose = null;
    }

    public virtual void OnCloseClick()
    {
        this.Close();
    }

    public virtual void OnYesClick()
    {
        this.Close(WindowResult.Yes);
    }

    public void OnMouseDown()
    {
        Debug.LogFormat(this.name + "Clicked");
    }
}
