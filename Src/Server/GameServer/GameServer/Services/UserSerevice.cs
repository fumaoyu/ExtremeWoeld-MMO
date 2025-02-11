using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Services
{
    class UserService : Singleton<UserService>
    {

        public UserService()//根据客户端的请求，注册事件
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCreateCharacter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameLeaveRequest>(this.OnGameLeave);
        }


        public void Init()
        {

        }

        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Log.InfoFormat("UserLoginRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userLogin = new UserLoginResponse();


            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user == null)
            {
                message.Response.userLogin.Result = Result.Failed;
                message.Response.userLogin.Errormsg = "用户不存在";
            }
            else if (user.Password != request.Passward)
            {
                message.Response.userLogin.Result = Result.Failed;
                message.Response.userLogin.Errormsg = "密码错误";
            }
            else
            {
                sender.Session.User = user;//--------------登录成功当前的会话用户

                message.Response.userLogin.Result = Result.Success;
                message.Response.userLogin.Errormsg = "None";
                message.Response.userLogin.Userinfo = new NUserInfo();
                message.Response.userLogin.Userinfo.Id = (int)user.ID;
                message.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                message.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                foreach (var c in user.Player.Characters)//登录返回当前数据库的所有角色
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = c.ID;
                    info.Name = c.Name;
                    info.Type = CharacterType.Player;
                    info.Class = (CharacterClass)c.Class;
                    info.Tid = c.ID;
                    message.Response.userLogin.Userinfo.Player.Characters.Add(info);//当前账号所有角色填充进这个协议发给客户端
                }

            }
            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }

        void OnRegister(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userRegister = new UserRegisterResponse();


            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();//
            if (user != null)////////////////一系列效验比较简单
            {
                message.Response.userRegister.Result = Result.Failed;
                message.Response.userRegister.Errormsg = "用户已存在.";
            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());//用户不存在新增加一个player，一个user对于一个player，一对一，player在前
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });//新增一个user
                DBService.Instance.Entities.SaveChanges();//保存新增的
                message.Response.userRegister.Result = Result.Success;
                message.Response.userRegister.Errormsg = "None";
            }

            byte[] data = PackageHandler.PackMessage(message);//消息打包成字节流
            sender.SendData(data, 0, data.Length);//发送给客户端
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="request">请求消息内容</param>
        private void OnCreateCharacter(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            Log.InfoFormat("UserCreateCharacterRequest: Name:{0}  Class:{1}", request.Name, request.Class);

            TCharacter character = new TCharacter()
            {
                Name = request.Name,
                Class = (int)request.Class,
                TID = (int)request.Class,
                MapID = 1,
                MapPosX = 5000,
                MapPosY = 4000,
                MapPosZ = 820,
            };

            var bag = new TCharacterBag();
            bag.Owner = character;
            bag.Items = new byte[0];
            bag.Unlocked = 20;
           // TCharacterItem it = new TCharacterItem();
            character.CharacterBag = DBService.Instance.Entities.CharacterBags.Add(bag);


            DBService.Instance.Entities.Characters.Add(character);//存入数据库
            sender.Session.User.Player.Characters.Add(character);//Session内存中的角色对象，每次登录用户信息都会保存在session，为了性能，不用每次都从DB中拉取
            DBService.Instance.Entities.SaveChanges();//保存

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.createChar = new UserCreateCharacterResponse();
            message.Response.createChar.Result = Result.Success;
            message.Response.createChar.Errormsg = "None";

            foreach (var c in sender.Session.User.Player.Characters)//把当前已经有 的角色添加进去返回
            {
                NCharacterInfo info = new NCharacterInfo();
                info.Id = 0;
                info.Name = c.Name;
                info.Type = CharacterType.Player;
                info.Class = (CharacterClass)c.Class;
                info.Tid = c.ID;
                message.Response.createChar.Characters.Add(info);
            }


            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);//sender就是客户端的那个session,因为服务端要发给很多人，要区别每一个用户session,跟客户端发送消息不一样


            //-----------------自己再写一次
            #region 自己再敲一次的
            //Log.InfoFormat("fedwsfs");
            //TCharacter characters = new TCharacter()
            //{
            //    Name = request.Name,
            //    Class = (int)request.Class,
            //    TID=(int)request.Class,
            //    MapID = 1,
            //    MapPosX = 5000,
            //    MapPosY = 4000,
            //    MapPosZ = 820,
            //};
            //DBService.Instance.Entities.Characters.Add(characters);//存入数据库
            //sender.Session.User.Player.Characters.Add(characters);
            //DBService.Instance.Entities.SaveChanges();

            //NetMessage netMessages = new NetMessage();
            //netMessages.Response = new NetMessageResponse();
            //netMessages.Response.createChar = new UserCreateCharacterResponse();
            //netMessages.Response.createChar.Result = Result.Success;
            //netMessages.Response.createChar.Errormsg = "None";

            //byte[] datas = PackageHandler.PackMessage(netMessages);
            //sender.SendData(datas, 0, data.Length);
            #endregion
        }

        private void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            //throw new NotImplementedException();
            TCharacter dbcharacter = sender.Session.User.Player.Characters.ElementAt(request.characterIdx);//得到当前选择的是第几个角色
            Log.InfoFormat("UserGameEnterRequest: charactetID :{0} :{1}  Map:{2}", dbcharacter.ID, dbcharacter.Name, dbcharacter .MapID);
            Character character = CharacterManager.Instance.AddCharacter(dbcharacter);//把角色添加到角色管理器，管理所有角色


            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.gameEnter = new UserGameEnterResponse();
            message.Response.gameEnter.Result = Result.Success;
            message.Response.gameEnter.Errormsg = "None";

            //进入成功，发送初始角色信息
            message.Response.gameEnter.Character = character.Info;

            //测试用例道具系统
            int itemid = 3;
            bool hasitem = character.ItemManager.HasItem(itemid);
            Log.InfoFormat("HasItem:[{0} {1}]", itemid, hasitem);
            if (hasitem)
            {
                // character.ItemManager.RemoveItem(itemid, 1);
            }
            else
            {
                character.ItemManager.AddItem(1, 4);
                character.ItemManager.AddItem(2, 100);

                character.ItemManager.AddItem(3, 25);
                character.ItemManager.AddItem(4, 120);
            }
            Models.Item item = character.ItemManager.GetItem(itemid);
            Log.InfoFormat("Item:[{0} {1}]", itemid, item);
            DBService.Instance.Save();

            byte[] date = PackageHandler.PackMessage(message);
            sender.SendData(date, 0, date.Length);
            sender.Session.Character = character;//当前哪个角色进行操作保存下来
            MapManager.Instance[dbcharacter.MapID].CharacterEnter(sender, character);//把自己告诉地图管理器我要进入地图了

        }

        private void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserGameLeaveRequest: charactetID :{0} :{1}  Map:{2}", character.Id, character.Info.Name, character.Info.mapId);

            CharacterLeave(character);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.gameLeave = new UserGameLeaveResponse();
            message.Response.gameLeave.Result = Result.Success;
            message.Response.gameLeave.Errormsg = "None";

            byte[] date = PackageHandler.PackMessage(message);
            sender.SendData(date, 0, date.Length);//给客户端发回
                                                  //sender.Session.Character = character;//

        }

        public   void CharacterLeave(Character character)
        {
            CharacterManager.Instance.RemoveCharacter(character.Id);//从角色管理器中拿出
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);//从地图中删除
        }

    

    }
}
