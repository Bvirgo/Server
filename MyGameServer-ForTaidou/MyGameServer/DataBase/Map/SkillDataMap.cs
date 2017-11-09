using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using MyGameServer.DataBase.Model;
using MyGameCommon.Model;

namespace MyGameServer.DataBase.Map 
{
    public class SkillDataMap :ClassMap<SkillData>
    {
        public SkillDataMap()
        {
            Id(x => x.ID).Column("id");
            Map(x => x.SkillID).Column("skillid");
            Map(x => x.Lv).Column("lv");
            Map(x => x.Pos).Column("pos");
            References(x => x.Role).Column("roleid");
            Table("skill");
        }
    }
}
