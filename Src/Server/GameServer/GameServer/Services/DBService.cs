using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Common;

namespace GameServer.Services
{
    class DBService : Singleton<DBService>
    {
        ExtremeWorldEntities entities;

        public ExtremeWorldEntities Entities
        {
            get { return this.entities; }
        }

        public void Init()
        {
            entities = new ExtremeWorldEntities();
           // _timer=new Timer(Save,)
        }

        //数据回档，为了避免短时间大量保存，性能问题，可以简单做个定时器，保存数据时间间隔
        float time = 0;
        private object _lock = new object();
        private Timer _timer;
        public void Save()
        {
            //DateTime time1 = DateTime.Now.AddSeconds(-5);

            //long diff = DateTime.Now.Ticks - time1.Ticks;
            //if (diff > 5)
            //{
            //    entities.SaveChangesAsync();//异步保存db数据
            //}
            //else
            //{

            //}
            entities.SaveChangesAsync();//异步保存db数据
        }
    }
}
