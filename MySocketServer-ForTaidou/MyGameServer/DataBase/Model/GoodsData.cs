using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGameCommon.Model;

namespace MyGameServer.DataBase.Model
{
    public class GoodsData
    {
        public virtual int ID { get; set; }
        public virtual int GoodsID { get; set; }
        public virtual int Num { get; set; }
        public virtual int Lv { get; set; }
        public virtual byte Drs { get; set; }
        public virtual Role Role { get; set; }

    }
}
