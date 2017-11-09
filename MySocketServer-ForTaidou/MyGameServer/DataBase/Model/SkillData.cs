using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameServer.DataBase.Model
{
    public class SkillData
    {
        public virtual int ID { get; set; }
        public virtual int SkillID { get; set; }
        public virtual int Lv { get; set; }
        public virtual int Pos { get; set; }
        public virtual Role Role { get; set; }

    }
}
