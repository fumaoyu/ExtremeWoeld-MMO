using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameServer;
using GameServer.Entities;
using GameServer.Network;
using GameServer.Services;
using SkillBridge.Message;

namespace Network
{
    class NetSession: INetSession
    {
        public TUser User { get; set; }
        public Character Character { get; set; }
        public NEntity Entity { get; set; }
        /// <summary>
        /// 后处理器
        /// </summary>
        public IPostResponser PostResponser { get; set; }
        /// <summary>
        /// 客户端断开之后删掉服务器的角色,角色离开
        /// </summary>
        public void Disconnected()
        {
            this.PostResponser = null;
            if (this.Character != null)
            {
                UserService.Instance.CharacterLeave(this.Character);
            }

        }
        NetMessage response;//根响应消息构建
        public NetMessageResponse Response
        {
            get
            {
                if (response == null)
                {
                    response = new NetMessage();
                }
                if (response.Response == null)
                {
                    response.Response = new NetMessageResponse();
                    
                }

                return response.Response;
            }
        }
       /// <summary>
       /// 得到消息字节流
       /// </summary>
       /// <returns></returns>
        public byte[] GetResponse()
        {
            if (response != null)//相应不为空
            {
                //if (this.Character != null && this.Character.StatusManager.HasStatus)
                //{
                //    this.Character.StatusManager.ApplyResponse(Response);
                //}
                if (PostResponser != null)
                    this.PostResponser.PostProcess(Response);
                /*
                if (this.Character!=null&&this.Character.StatusManager.HasStatus)
                {
                    this.Character.StatusManager.ApplyResponse(Response);
                }*/
                byte[] data = PackageHandler.PackMessage(response);
                response = null;//一个根消息打包发送之后为空，下一次不会冲突，下次是重新创建一个，会话开始创建，会话结束为空
                return data;
            }
            return null;
        }


    }
}
