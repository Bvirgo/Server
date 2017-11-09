using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using MyGameCommon.Model;

namespace MyGameServer.DataBase.Map
{
    public class ServerPropertyMap:ClassMap<ServerProperty>
    {
        public ServerPropertyMap()
        {
            Id(x => x.ID).Column("id");
            Map(x => x.IP).Column("ip");
            Map(x => x.Name).Column("name");
            Map(x => x.Count).Column("count");
            Table("serverproperty");
        }
    }
}
