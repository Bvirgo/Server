using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using MyGameServer.DataBase.Model;
using MyGameCommon.Model;

namespace MyGameServer.DataBase.Map
{
    public class GoodsListMap:ClassMap<GoodsData>
    {
        public GoodsListMap()
        {
            Id(x => x.ID).Column("id");
            Map(x => x.GoodsID).Column("goodsid");
            Map(x => x.Num).Column("num");
            Map(x => x.Lv).Column("lv");
            Map(x => x.Drs).Column("dress");

            References(x => x.Role).Column("roleid");

            Table("goodslist");
        }
    }
}
