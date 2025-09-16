using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Configuration;

using System.Threading;

using Network;
using GameServer.Services;
using GameServer.Managers;
using Services;

namespace GameServer
{
    class GameServer
    {
        Thread thread;
        bool running = false;
        NetService network;

        public bool Init()
        {
            int Port = Properties.Settings.Default.ServerPort;//配置好的端口和ip
            network = new NetService();
            network.Init(Port);
            DBService.Instance.Init();
            UserService.Instance.Init();
            
            DataManager.Instance.Load();
          //  MapManager.Instance.Init();

            MapService.Instance.Init();
            //测试
            BagService.Instance.Init();
            ItemService.Instance.Init();

            QuestService.Instance.Init();
           // HelloService.Instance.Init();
            HelloService.Instance.Init();

            FriendService.Instance.Init();
            TeamService.Instance.Init();
            GuildService.Instance.Init();
            ChatService.Instance.Init();
            BattleServive.Instance.Init();
            thread = new Thread(new ThreadStart(this.Update));
            return true;
        }

        public void Start()
        {
            network.Start();
            running = true;
            thread.Start();
            HelloService.Instance.Start();
        }


        public void Stop()
        {
            running = false;
            thread.Join();
            network.Stop();
        }

        public void Update()
        {
            while (running)
            {
                Time.Tick();
                Thread.Sleep(100);
                MapManager.Instance.Update();
                //Console.WriteLine("{0} {1} {2} {3} {4}", Time.deltaTime, Time.frameCount, Time.ticks, Time.time, Time.realtimeSinceStartup);
            }
        }
    }
}
