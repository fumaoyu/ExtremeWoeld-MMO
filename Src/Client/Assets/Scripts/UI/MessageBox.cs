using UnityEngine;

class MessageBox
{
    static Object cacheObject = null;

    public static UIMessageBox Show(string message, string title="", MessageBoxType type = MessageBoxType.Information, string btnOK = "", string btnCancel = "")
    {
        if(cacheObject==null)
        {
            cacheObject = Resloader.Load<Object>("UI/UIMessageBox");
        }

        GameObject go = (GameObject)GameObject.Instantiate(cacheObject);
        UIMessageBox msgbox = go.GetComponent<UIMessageBox>();
        msgbox.Init(title, message, type, btnOK, btnCancel);
        return msgbox;
    }
}

public enum MessageBoxType
{
    /// <summary>
    /// Information Dialog with OK button信息提示
    /// </summary>
    Information = 1,

    /// <summary>
    /// Confirm Dialog whit OK and Cancel buttons确认提示
    /// </summary>
    Confirm = 2,

    /// <summary>
    /// Error Dialog with OK buttons错误提示
    /// </summary>
    Error = 3
}