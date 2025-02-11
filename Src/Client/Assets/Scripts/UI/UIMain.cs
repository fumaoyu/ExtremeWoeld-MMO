using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain>
{
    // Start is called before the first frame update

    public Text avaterName;
    public Text avaterLV;
    protected override void OnStart()
    {
        this.UpdateAvater();
    }


    void UpdateAvater()
    {
        this.avaterName.text = string.Format("{0} [{1}]", User.Instance.CurrentCharacter.Name, User.Instance.CurrentCharacter.Id);
        this.avaterLV.text = User.Instance.CurrentCharacter.Level.ToString();
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void OnBackCharacterSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");
        Services.UserService.Instance.SendGameLeave();
    }

    public void OnTestClick()
    {
        UITest ui = UIManager.Instance.Show<UITest>();
        ui.UpdateText("侧解密");
        ui.OnClose += (send, res) => { MessageBox.Show("测试成功"+res, "消息提示", MessageBoxType.Confirm); };
    }

    public void OnClickBag()
    {
        UIManager.Instance.Show<UIBag>();
    }
}
