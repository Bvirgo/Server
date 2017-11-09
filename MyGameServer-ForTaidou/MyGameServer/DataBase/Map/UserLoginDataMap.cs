using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using MyGameServer.DataBase.Model;

namespace MyGameServer.DataBase.Map
{
    public class UserLoginDataMap:ClassMap<UserLoginData>
    {
        public UserLoginDataMap()
        {
            Id(x => x.Id).Column("id");
            Map(x => x.Name).Column("name");
            Map(x => x.Password).Column("password");
            Table("user");
        }
    }
}
