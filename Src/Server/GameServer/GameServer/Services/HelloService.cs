using Common;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class HelloService:Singleton<HelloService>
    {

        public void Init()
        {
            // entities = new ExtremeWorldEntities();
        }

        public void Stop()
        {
        }
        public void     Start()
        {

            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FirstTestRequest>(this.OnFirstTestRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<SenondTestRequest>(this.OnsecondeRequest);
        }

        private void OnsecondeRequest(NetConnection<NetSession> sender, SenondTestRequest message)
        {
            Log.InfoFormat("SecobdRequest:第二次测试:{0}, ", message.Id);
            sender.Session.Response.SenondTest = new SenondTestRespose();
            sender.Session.Response.SenondTest.Result = Result.Success;
            sender.Session.Response.SenondTest.sName = "付茂宇你成功了，但是你还得努力找工作";
            sender.SendResponse();
        }

        private void OnFirstTestRequest(NetConnection<NetSession> sender, FirstTestRequest message)
        {
            Log.InfoFormat("UserLoginRequest:HeoolWorld:{0}, ", message.Helloworld);
           
            //FriendAddRequest ds = new FriendAddRequest();
            //ds.FromName = "fd";
            //row new NotImplementedException();
        }
    }
}
