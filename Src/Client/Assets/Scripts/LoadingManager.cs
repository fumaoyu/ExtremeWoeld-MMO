using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

using SkillBridge.Message;
using ProtoBuf;
using Services;
using Managers;
using Network;

public class LoadingManager : MonoBehaviour {

    public GameObject UITips;
    public GameObject UILoading;
    public GameObject UILogin;

    public Slider progressBar;
    public Text progressText;
    public Text progressNumber;

    // Use this for initialization
    IEnumerator Start()
    {
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));//初始化日志系统
        UnityLogger.Init();
        Common.Log.Init("Unity");
        Common.Log.Info("LoadingManager start");

        UITips.SetActive(true);
        UILoading.SetActive(false);
        UILogin.SetActive(false);
        yield return new WaitForSeconds(2f);
        UILoading.SetActive(true);
        //yield return new WaitForSeconds(1f);
        UITips.SetActive(false);

        yield return DataManager.Instance.LoadData();//数据加载

        //Init basic services
        MapService.Instance.Init();
        UserService.Instance.Init();
        TaskManager.Instance.Init();
        StatusService.Instance.Init();
        ShopManager.Instance.Init();

        FriendService.Instance.Init();

        TeamService.Instance.Init();

        GuildService.Instance.Init();//公会

        ChatService.Instance.Init();

        BattleServive.Instance.Init();
        AudioManager.Instance.PlayMusic(SoundDefine.Music_Login);
        // Fake Loading Simulate
        progressBar.value = 0;
        for (float j = 0; j <100;j++)
        {
            float i = Random.Range(0f,0.3f);
            if (progressBar.value >= 1)
            {
                progressBar.value = 1;
                yield return new WaitForSeconds(0.3f);
                break;
            }
           // Debug.Log("j" + j);
                //Debug.Log("进度条" + );
                //Debug.Log("进度条" + );
            progressBar.value += i;
            progressNumber.text =((int)(progressBar.value*1000)/10)+","+ ((int)(progressBar.value * 1000)%10) + "%";
            yield return new WaitForSeconds(Random.Range(0f,0.3f));
        }
        UILoading.SetActive(false);
        UILogin.SetActive(true);
        yield return null;
    }


    // Update is called once per frame
    void Update () {

    }
}
