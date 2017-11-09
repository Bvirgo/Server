using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameServer.DataBase.Model
{
    public class RoleInfoData
    {
        public virtual int ID { get; set; }
        public virtual int Exp { get; set; }
        public virtual int Gem { get; set; }
        public virtual int Gold { get; set; }
        public virtual int Energy { get; set; }
        public virtual int Toughen { get; set; }
        public virtual Role Role { get; set; }
    }
}
