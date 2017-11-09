using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using MyGameServer.DataBase.Model;
using MyGameCommon.Model;

namespace MyGameServer.DataBase.Map
{
    public class EquipListMap:ClassMap<EquipData>
    {
        public EquipListMap()
        {
            Id(x => x.ID).Column("id");
            Map(x => x.GoodsID).Column("goodsid");
            Map(x => x.Lv).Column("lv");

            References(x => x.Role).Column("roleid");

            Table("equiplist");
        }
    }
}
