using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGameCommon.Model;

namespace MyGameServer.DataBase.Manager
{
    public class ServerPropertyMgr
    {
        // 获取表中所有数据
        public List<ServerProperty> GetAllServer()
        {
            // 获取数据库Session
            using (var session = NHibernateHelper.OpenSession())
            {
                // 获取视图
                using (var transaction = session.BeginTransaction())
                {
                    var userList = session.QueryOver<ServerProperty>();
                    // 提交查询
                    transaction.Commit();
                    return (List<ServerProperty>)userList.List();
                }
            }
        }


        // 条件查询：根据用户名查询
        public IList<ServerProperty> GetServerByName(string _strName)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    // 条件查询：这个user是ServerProperty对象
                    var userList = session.QueryOver<ServerProperty>().Where(user => user.Name == _strName);
                    transaction.Commit();
                    return userList.List();
                }
            }
        }

        // 条件查询：根据用户名查询
        public List<ServerProperty> GetServerByID(int _nID)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    // 条件查询：这个user是ServerProperty对象
                    var userList = session.QueryOver<ServerProperty>().Where(user => user.ID == _nID);
                    transaction.Commit();
                    return (List<ServerProperty>)userList.List();
                }
            }
        }

        // 数据保存
        public void SaveServer(ServerProperty user)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(user);
                    transaction.Commit();
                }
            }
        }     

        // 数据库信息更新:传入修改好的数据，如果该数据已经存在就更新，如果没有就添加
        public void UpdateServer(ServerProperty tu)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Update(tu);
                    transaction.Commit();
                }
            }
        }
    }
}
