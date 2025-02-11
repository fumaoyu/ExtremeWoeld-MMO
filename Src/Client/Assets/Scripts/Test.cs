using Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Network.NetClient.Instance.Init("127.0.0.1", 8000);
        NetClient.Instance.Connect();
        SkillBridge.Message.NetMessage msg=new SkillBridge.Message.NetMessage();//主消息

        SkillBridge.Message.FirstTestRequest firstTestRequest  =new SkillBridge.Message.FirstTestRequest();//这是子消息自己的
        msg.Request = new SkillBridge.Message.NetMessageRequest();//请求消息
        //msg.Request.firstRequest = firstTestRequest;
       // firstTestRequest.Helloworld = "Hello Woele";
        Network.NetClient.Instance.SendMessage(msg);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
