using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGameCommon.Model;
using MyGameServer.DataBase.Model;

namespace MyGameServer.DataBase.Manager
{
    public class RoleMgr
    {
        // 获取用户在该服务器中所有角色
        public List<Role> GetUserRole(int _userID,int _serverID)
        {
            // 获取数据库Session
            using (var session = NHibernateHelper.OpenSession())
            {
                // 获取视图
                using (var transaction = session.BeginTransaction())
                {
                    var userList = session.QueryOver<Role>().Where(x => x.User.Id == _userID && x.Server.ID == _serverID );
                    // 提交查询
                    transaction.Commit();
                    return (List<Role>)userList.List();
                }
            }
        }

        // 获取该服务器中的所有角色，新建角色防止重名
        public List<Role> GetServerRole(int _serverID)
        {
            // 获取数据库Session
            using (var session = NHibernateHelper.OpenSession())
            {
                // 获取视图
                using (var transaction = session.BeginTransaction())
                {
                    var userList = session.QueryOver<Role>().Where(x => x.Server.ID == _serverID);
                    // 提交查询
                    transaction.Commit();
                    return (List<Role>)userList.List();
                }
            }
        }

        // 根据用户名获取同服中的指定角色
        public List<Role> GetRoleByName(string _name,int _serverID)
        {
            // 获取数据库Session
            using (var session = NHibernateHelper.OpenSession())
            {
                // 获取视图
                using (var transaction = session.BeginTransaction())
                {
                    var userList = session.QueryOver<Role>().Where(x => x.Name == _name && x.Server.ID == _serverID);
                    // 提交查询
                    transaction.Commit();
                    return (List<Role>)userList.List();
                }
            }
        }

        // 数据保存
        public void SaveRole(Role user)
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

        // 条件删除:主键ID
        public void DeleteById(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    Role tu = new Role();
                    tu.Id = id;
                    session.Delete(tu);
                    transaction.Commit();
                }
            }
        }

        // 数据库信息更新:传入修改好的数据，如果该数据已经存在就更新，如果没有就添加
        public void UpdateUser(Role tu)
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
