using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;

using SkillBridge.Message;
using Models;
using Managers;

namespace Services
{
    class UserService : Singleton<UserService>, IDisposable
    {

        public UnityEngine.Events.UnityAction<Result, string> OnLogin;
        public UnityEngine.Events.UnityAction<Result, string> OnRegister;
        public UnityEngine.Events.UnityAction<Result, string> OnCharacterCreate;

        /// <summary>
        /// 用来保存消息的类似队列
        /// </summary>
        NetMessage pendingMessage = null;

        bool connected = false;

        bool isQuitGame = false;
        public UserService()//响应事件注册，根据服务器返回来的消息
        {
            NetClient.Instance.OnConnect += OnGameServerConnect;//游戏服务器连上
            NetClient.Instance.OnDisconnect += OnGameServerDisconnect;//游戏服务器断开
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Subscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Subscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
            MessageDistributer.Instance.Subscribe<UserGameEnterResponse>(this.OnGameEnter);
            MessageDistributer.Instance.Subscribe<UserGameLeaveResponse>(this.OnGameLeave);
            //MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnCharacterEnter);
            MessageDistributer.Instance.Subscribe<SenondTestRespose>(this.OnSecondTest);
        }

        

        public void Dispose()//一般成对出现订阅和取消订阅
        {
            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Unsubscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
            MessageDistributer.Instance.Unsubscribe<UserGameEnterResponse>(this.OnGameEnter);
             MessageDistributer.Instance.Unsubscribe<UserGameLeaveResponse>(this.OnGameLeave);
          //  MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnCharacterEnter);



            NetClient.Instance.OnConnect -= OnGameServerConnect;
            NetClient.Instance.OnDisconnect -= OnGameServerDisconnect;
        }

        public void Init()
        {
            //NetMessage msg = new NetMessage();
            //msg.Request = new NetMessageRequest();
            //msg.Request.secodnTestRequest = new SenondTestRequest();
            //msg.Request.secodnTestRequest.Id = 1;
            //NetClient.Instance.SendMessage(msg);
        }

        /// <summary>
        /// 断线重连连接服务器
        /// </summary>
        public void ConnectToServer()
        {
            Debug.Log("ConnectToServer() Start ");
            //NetClient.Instance.CryptKey = this.SessionId;
            NetClient.Instance.Init("127.0.0.1", 8000);
            NetClient.Instance.Connect();
        }


        void OnGameServerConnect(int result, string reason)
        {
            Log.InfoFormat("LoadingMesager::OnGameServerConnect :{0} reason:{1}", result, reason);
            if (NetClient.Instance.Connected)
            {
                this.connected = true;//连接成功
                if(this.pendingMessage!=null)//判断连接成功前有没有要发的消息有就把消息发出去
                {
                    NetClient.Instance.SendMessage(this.pendingMessage);//pendingMessage类似一个队列存储消息
                    this.pendingMessage = null;
                }
            }
            else
            {
                if (!this.DisconnectNotify(result, reason))
                {
                    MessageBox.Show(string.Format("网络错误，无法连接到服务器！\n RESULT:{0} ERROR:{1}", result, reason), "错误", MessageBoxType.Error);
                }
            }
        }

        public void OnGameServerDisconnect(int result, string reason)
        {
            this.DisconnectNotify(result, reason);
            return;
        }

        bool DisconnectNotify(int result,string reason)
        {
            if (this.pendingMessage != null)
            {
                if (this.pendingMessage.Request.userLogin!=null)
                {
                    if (this.OnLogin != null)
                    {
                        this.OnLogin(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                else if(this.pendingMessage.Request.userRegister!=null)
                {
                    if (this.OnRegister != null)
                    {
                        this.OnRegister(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                else
                {
                    if (this.OnCharacterCreate != null)
                    {
                        this.OnCharacterCreate(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                return true;
            }
            return false;
        }

        public void SendLogin(string user, string psw)
        {
            Debug.LogFormat("UserLoginRequest::user :{0} psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userLogin = new UserLoginRequest();
            message.Request.userLogin.User = user;
            message.Request.userLogin.Passward = psw;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }

        
        }

        void OnUserLogin(object sender, UserLoginResponse response)
        {
            Debug.LogFormat("OnLogin:{0} [{1}]", response.Result, response.Errormsg);

            if (response.Result == Result.Success)
            {//登陆成功逻辑
                Models.User.Instance.SetupUserInfo(response.Userinfo);//在本地有一个user，为了不用每次都从服务器拉取数据要是数据不变的话，可以随时获取用户信息
            };
            if (this.OnLogin != null)
            {
                this.OnLogin(response.Result, response.Errormsg);//调用事件

            }
        }


        public void SendRegister(string user, string psw)
        {
            Debug.LogFormat("UserRegisterRequest::user :{0} psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userRegister = new UserRegisterRequest();
            message.Request.userRegister.User = user;
            message.Request.userRegister.Passward = psw;

            if (this.connected && NetClient.Instance.Connected)//判断连接有没有连上
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;//这个pendingMessage是一个类似一个队列先把消息记下来连上了再发出去
                this.ConnectToServer();//没有连上重连
            }
        }

        void OnUserRegister(object sender, UserRegisterResponse response)
        {
            Debug.LogFormat("OnUserRegister:{0} [{1}]", response.Result, response.Errormsg);

            if (this.OnRegister != null)//事件不为空
            {
                this.OnRegister(response.Result, response.Errormsg);//调用时间

            }
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="name">角色名字</param>
        /// <param name="cls">职业</param>
        public void SendCharacterCreate(string name, CharacterClass cls)
        {
            Debug.LogFormat("SenderCharacterCreate:{0} {1}",name,cls);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.createChar = new UserCreateCharacterRequest();
            message.Request.createChar.Name = name;
            message.Request.createChar.Class = cls;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }

            //----------------------------自己再敲一次
            #region 自己再写一次
            
            //Debug.LogFormat("user:{0},djus ：{1}",name,cls);
            //NetMessage netMessage = new NetMessage();
            //netMessage.Request = new NetMessageRequest();
            //netMessage.Request.createChar = new UserCreateCharacterRequest();
            //netMessage.Request.createChar.Class = cls;
            //netMessage.Request.createChar.Name = name;
            //if (this.connected && NetClient.Instance.Connected)
            //{
            //    this.pendingMessage = null;
            //    NetClient.Instance.SendMessage(message);
            //}
            //else
            //{
            //    this.pendingMessage = message;
            //    this.ConnectToServer();
            //}
            #endregion
        }

        /// <summary>
        /// 接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        void OnUserCreateCharacter(object sender, UserCreateCharacterResponse response)
        {
            Debug.LogFormat("OnUserCreateCharacter:{0} [{1}]", response.Result, response.Errormsg);

            if(response.Result == Result.Success)
            {
                Models.User.Instance.Info.Player.Characters.Clear();
                Models.User.Instance.Info.Player.Characters.AddRange(response.Characters);//把返回的角色列表填进去

            }

            if (this.OnCharacterCreate != null)
            {
                this.OnCharacterCreate(response.Result, response.Errormsg);

            }
            //---------------------自己再敲一次
            #region 再敲一次
            //if (response.Result == Result.Success)
            //{

            //}

            #endregion
        }
        public void SendGameEnter(int characterId)
        {
            Debug.LogFormat("SendGameEnter:characterId:{0}", characterId);
            NetMessage netMessage = new NetMessage();
            netMessage.Request = new NetMessageRequest();
            netMessage.Request.gameEnter = new UserGameEnterRequest();


          ChatManager.Instance.Init();///退出场景在回来的时候


            netMessage.Request.gameEnter.characterIdx = characterId;
            if (this.connected && NetClient.Instance.Connected)
            { 
                this.pendingMessage = null;//等待发送的信息列表
                NetClient.Instance.SendMessage(netMessage);
            }
            else
            {
                this.pendingMessage = netMessage;
                this.ConnectToServer();
            }
            //NetMessage msg = new NetMessage();
            //msg.Request = new NetMessageRequest();
           
            //NetClient.Instance.SendMessage(netMessage);
        }

        private void OnGameEnter(object sender, UserGameEnterResponse message)
        {
            Debug.LogFormat("收到进入游戏：{0}  玩家Id{1}  名字：{2}", message.Result,message.Character.Id,message.Character.Name);
            if (message.Result == Result.Success)
            {
                if (message.Character != null)
                {
                    User.Instance.CurrentCharacterInfo = message.Character;

                    ItemManager.Instance.Init(message.Character.Items);//初始化道具
                    BagManager.Instance.Init(message.Character.Bag);//初始化背包
                    EquipManager.Instance.Init(message.Character.Equips);
                    QuestManager.Instance.Init(message.Character.Quests);

                    FriendManager.Instance.Init(message.Character.Friends);

                    GuildManager.Instance.Init(message.Character.Guild);
                }
            }
        }

        private void OnCharacterEnter(object sender, MapCharacterEnterResponse message)
        {
            //throw new NotImplementedException();
            Debug.LogFormat("MapCharacterEnter: {0}", message.mapId);
            Debug.LogFormat("当前玩家信息 名字:{0}  Id:{1}", User.Instance.CurrentCharacterInfo.Name, User.Instance.CurrentCharacterInfo.Id);
            NCharacterInfo info = message.Characters[0];//因为返回的就是当前一个角色

            Debug.LogFormat("当前玩家信息 名字:{0}  Id:{1}", info.Name, info.Id);

            User.Instance.CurrentCharacterInfo = info; 
            SceneManager.Instance.LoadScene(DataManager.Instance.Maps[message.mapId].Resource);
            AudioManager.Instance.PlayMusic(DataManager.Instance.Maps[message.mapId].Music);
        }
        void OnSecondTest(object sender, SenondTestRespose message)
        {
            Debug.LogFormat("OnSecondTestResponse {0}|| 结果  {1}" , message.sName, message.Result);
        }


        public void SendGameLeave(bool isquitGAME=false)
        {
            this.isQuitGame = isquitGAME;

            Debug.LogFormat("UserGameLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.gameLeave = new UserGameLeaveRequest();

          
            NetClient.Instance.SendMessage(message);

            //NetMessage message = new NetMessage();
            //message.Request = new NetMessageRequest();
        }

        public void SendTestSecond()
        {
            //Debug.LogFormat("SecondTestRequest");
            //NetMessage message = new NetMessage();
            //message.Request = new NetMessageRequest();
            ////message.Request.secodnTestRequest = new SenondTestRequest();
            ////message.Request.secodnTestRequest.Id = 1;

            ////message.Request.secodnTestRequest = new SenondTestRequest();
            ////message.Request.secodnTestRequest.Id = 1;

            //NetClient.Instance.SendMessage(message);

            //NetMessage message = new NetMessage();
            //message.Request = new NetMessageRequest();
        }

        private void OnGameLeave(object sender, UserGameLeaveResponse message)
        {
            Debug.LogFormat("OnGameLeave: {0}  [{1}] ", message.Result, message.Errormsg);
            //离开地图id重新设置为零
            MapService.Instance.CurrentMapId = 0;
            User.Instance.CurrentCharacterInfo = null;
            User.Instance.CurrentCharacter = null;
            if (this.isQuitGame)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
              Application.Quit();////打包之后
#endif
            }
        }


    }
}
