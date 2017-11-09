using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGameCommon.Model;
using MyGameServer.DataBase.Model;

namespace MyGameServer.DataBase.Manager
{
    public class GoodsListMgr
    {
        /// <summary>
        /// 获取指定角色包裹列表
        /// </summary>
        /// <param name="_roleID"></param>
        /// <returns></returns>
        public List<GoodsData> GetGoodsList(int _roleID)
        {
            // 获取数据库Session
            using (var session = NHibernateHelper.OpenSession())
            {
                // 获取视图
                using (var transaction = session.BeginTransaction())
                {
                    var infoList = session.QueryOver<GoodsData>().Where(x => x.Role.Id == _roleID);
                    // 提交查询
                    transaction.Commit();
                    return (List<GoodsData>)infoList.List();
                }
            }
        }

        /// <summary>
        /// 根据goods数据库中唯一ID获取物品,不能是物品ID
        /// </summary>
        /// <param name="_id"></param>
        /// <returns></returns>
        public List<GoodsData> GetGoodsByID(int _id)
        {
            // 获取数据库Session
            using (var session = NHibernateHelper.OpenSession())
            {
                // 获取视图
                using (var transaction = session.BeginTransaction())
                {
                    var infoList = session.QueryOver<GoodsData>().Where(x =>x.ID == _id);
                    // 提交查询
                    transaction.Commit();
                    return (List<GoodsData>)infoList.List();
                }
            }
        }


        /// <summary>
        /// 删除物品
        /// </summary>
        /// <param name="_id"></param>
        public void DeleteGoods(GoodsData goods)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Delete(goods);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// 添加一个角色包裹列表:返回新物品主键
        /// </summary>
        /// <param name="_rolfInfo"></param>
        public int AddGoodsList(GoodsData _goodsInfo)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    int newGoodsID = (int)session.Save(_goodsInfo);
                    transaction.Commit();
                    return newGoodsID;
                }
            }
        }

        /// <summary>
        /// 更新角色包裹列表
        /// </summary>
        /// <param name="_rolfInfo"></param>
        public void UpdateGoodsList(GoodsData _goodsInfo)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Update(_goodsInfo);
                    transaction.Commit();
                }
            }
        }
    }
}
