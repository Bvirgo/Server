using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using MyGameServer.DataBase.Model;
using MyGameCommon.Model;

namespace MyGameServer.DataBase.Map
{
    public class RoleInfoMap : ClassMap<RoleInfoData>
    {
        public RoleInfoMap()
        {
            Id(x => x.ID).Column("id");
            Map(x => x.Exp).Column("exp");
            Map(x =>x.Gem).Column("gem");
            Map(x => x.Gold).Column("gold");
            Map(x => x.Energy).Column("energy");
            Map(x => x.Toughen).Column("toughen");

            References(x => x.Role).Column("roleid");

            Table("roleinfo");

        }
    }
}
