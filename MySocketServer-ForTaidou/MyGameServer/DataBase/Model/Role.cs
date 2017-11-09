using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGameCommon.Model;

namespace MyGameServer.DataBase.Model
{
    public class Role
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int Lv { get; set; }
        public virtual bool IsMan { get; set; }

        // 职业
        public virtual string Occup { get; set; }

        // 角色所属账号
        public virtual User User { get; set; }

        // 角色所属服务器
        public virtual ServerProperty Server { get; set; }

    }
}
