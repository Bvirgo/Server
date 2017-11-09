using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGameCommon.Model;
using MyGameServer.DataBase.Model;

namespace MyGameServer.DataBase.Manager
{
    public class RoleInfoMgr
    {

        /// <summary>
        /// 获取指定角色属性列表
        /// </summary>
        /// <param name="_roleID"></param>
        /// <returns></returns>
        public List<RoleInfoData> GetRoleData(int _roleID)
        {
            // 获取数据库Session
            using (var session = NHibernateHelper.OpenSession())
            {
                // 获取视图
                using (var transaction = session.BeginTransaction())
                {
                    var infoList = session.QueryOver<RoleInfoData>().Where(x => x.Role.Id == _roleID);
                    // 提交查询
                    transaction.Commit();
                    return (List<RoleInfoData>)infoList.List();
                }
            }
        }

        /// <summary>
        /// 添加一个角色属性列表
        /// </summary>
        /// <param name="_rolfInfo"></param>
        public void AddRoleInfo(RoleInfoData _rolfInfo)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(_rolfInfo);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// 更新角色属性列表
        /// </summary>
        /// <param name="_rolfInfo"></param>
        public void UpdateRoleInfo(RoleInfoData _rolfInfo)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Update(_rolfInfo);
                    transaction.Commit();
                }
            }
        }
    }
}
