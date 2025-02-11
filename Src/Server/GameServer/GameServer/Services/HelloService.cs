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
        }

        private void OnFirstTestRequest(NetConnection<NetSession> sender, FirstTestRequest message)
        {
            Log.InfoFormat("UserLoginRequest:HeoolWorld:{0}, ", message.Helloworld);
            //row new NotImplementedException();
        }
    }
}
