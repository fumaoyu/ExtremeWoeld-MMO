using Entities;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIMain : MonoSingleton<UIMain>
{
    // Start is called before the first frame update

    public Text avaterName;
    public Text avaterLV;
    public UITeam TeamWindow;
    public UIChat UIChat;

    public Button chatBtn;
    public Button teamBtn;

    public UICreatureInfo targetUI;

    public UISkillSlots skillSlots;

    protected override void OnStart()
    {
        this.UpdateAvater();
        
        UIChat.gameObject.SetActive(false);

        this.targetUI.gameObject.SetActive(false);
        BattleManager.Instance.OnTargetChanger += OnTargetChanger;
        User.Instance.OnCharacter += this.skillSlots.UpdateSkills;
        this.skillSlots.UpdateSkills();
    
    }


    void UpdateAvater()
    {
        this.avaterName.text = string.Format("{0} [{1}]", User.Instance.CurrentCharacterInfo.Name, User.Instance.CurrentCharacterInfo.Id);
        this.avaterLV.text = User.Instance.CurrentCharacterInfo.Level.ToString();
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
        ui.OnClose += (send, res) => { MessageBox.Show("测试成功" + res, "消息提示", MessageBoxType.Confirm); };
    }

    public void OnClickBag()
    {
        UIManager.Instance.Show<UIBag>();
    }

    public void OnClickEquip()
    {
        UIManager.Instance.Show<UICharEquip>();
    }
    public void OnClickQuest()
    {
        UIManager.Instance.Show<UIQuestSystem>();
    }

    public void OnClickFriend()
    {
        UIManager.Instance.Show<UIFriend>();
    }


    public void ShowTeamUI(bool show)
    {
        TeamWindow.ShowTeam(show);
    }

    public void OnShowGuild()
    {
        GuildManager.Instance.ShowGuild();
    }

    public void OnClickSetting()
    {
        UIManager.Instance.Show<UISetting>();
    }

    public void OnClickSkill()
    {
        UIManager.Instance.Show<UISkill>();
    }

    public void OnClickChat()
    {
        //UIManager.Instance.Show<UIChat>();
        ShowChatUI(true);
        chatBtn.gameObject.SetActive(false);
        UIChat.OnClose += (uichat, WindowResult) =>
        {
            chatBtn.gameObject.SetActive(true);
            ShowChatUI(false);
        };
    }

    public void ShowChatUI(bool show)
    {
        UIChat.ShowChatUI(show);
        //chatBtn.gameObject.SetActive(show);
    }

    public void OnClickRide()
    {
        UIManager.Instance.Show<UIRide>();
    }


    private void OnTargetChanger(Creature target)
    {
        if(target != null)
            {
            if (!targetUI.isActiveAndEnabled)
            {
                targetUI.gameObject.SetActive(true);
                
            }
            targetUI.TartGet = target;
        }

        else
        {
            targetUI.gameObject.SetActive(false);///没有目标显示隐藏
        }

    }

}