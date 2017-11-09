using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGameServer.DataBase.Model;

namespace MyGameServer.DataBase.Manager
{
    public class UserLoginDataMgr
    {
        // 条件查询：根据用户名查询
        public List<UserLoginData> GetUserByName(string _strName)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    // 条件查询
                    var userList = session.QueryOver<UserLoginData>().Where(user => user.Name == _strName);
                    transaction.Commit();
                    return (List <UserLoginData>)userList.List();
                }
            }
        }

        public List<UserLoginData> GetUserByID(int _nID)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    // 条件查询
                    var userList = session.QueryOver<UserLoginData>().Where(user => user.Id == _nID);
                    transaction.Commit();
                    return (List<UserLoginData>)userList.List();
                }
            }
        }

        // 数据保存
        public int AddUser(UserLoginData user)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    int nID = (int)session.Save(user);
                    transaction.Commit();
                    return nID;
                }
            }
        }

    }
}
