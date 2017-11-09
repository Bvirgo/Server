using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGameCommon.Model;
using MyGameServer.DataBase.Model;

namespace MyGameServer.DataBase.Manager
{
    public class EquipListMgr
    {
        /// <summary>
        /// 获取指定角色装备列表
        /// </summary>
        /// <param name="_roleID"></param>
        /// <returns></returns>
        public List<EquipData> GetEquipList(int _roleID)
        {
            // 获取数据库Session
            using (var session = NHibernateHelper.OpenSession())
            {
                // 获取视图
                using (var transaction = session.BeginTransaction())
                {
                    var infoList = session.QueryOver<EquipData>().Where(x => x.Role.Id == _roleID);
                    // 提交查询
                    transaction.Commit();
                    return (List<EquipData>)infoList.List();
                }
            }
        }

        public List<EquipData> GetEquipDataByID(int _id)
        {
            // 获取数据库Session
            using (var session = NHibernateHelper.OpenSession())
            {
                // 获取视图
                using (var transaction = session.BeginTransaction())
                {
                    var infoList = session.QueryOver<EquipData>().Where(x => x.GoodsID == _id);
                    // 提交查询
                    transaction.Commit();
                    return (List<EquipData>)infoList.List();
                }
            }
        }

        /// <summary>
        /// 添加一个角色装备列表
        /// </summary>
        /// <param name="_rolfInfo"></param>
        public void AddEquipList(EquipData _equipInfo)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(_equipInfo);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// 更新角色装备列表
        /// </summary>
        /// <param name="_rolfInfo"></param>
        public void UpdateEquipList(EquipData _equipInfo)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Update(_equipInfo);
                    transaction.Commit();
                }
            }
        }
    }
}
