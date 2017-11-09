using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using MyGameServer.DataBase.Model;
using MyGameCommon.Model;

namespace MyGameServer.DataBase.Map
{
    public class RoleDataMap:ClassMap<RoleData>
    {
        public RoleDataMap()
        {
            Id(x => x.Id).Column("id");
            Map(x => x.Name).Column("name");
            Map(x => x.Lv).Column("lv");
            Map(x => x.IsMan).Column("sex");
            Map(x => x.Occup).Column("occup");
            // 外键映射
            References(x => x.User).Column("userid");
            References(x => x.Server).Column("serverid");
            Table("role");
               
        }
    }
}
