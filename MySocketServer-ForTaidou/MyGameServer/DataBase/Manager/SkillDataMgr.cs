using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGameCommon.Model;
using MyGameServer.DataBase.Model;

namespace MyGameServer.DataBase.Manager
{
    public class SkillDataMgr
    {
        /// <summary>
        /// 获取指定角色技能列表
        /// </summary>
        /// <param name="_roleID"></param>
        /// <returns></returns>
        public List<SkillData> GetSkillData(int _roleID)
        {
            // 获取数据库Session
            using (var session = NHibernateHelper.OpenSession())
            {
                // 获取视图
                using (var transaction = session.BeginTransaction())
                {
                    var list = session.QueryOver<SkillData>().Where(x => x.Role.Id == _roleID);
                    // 提交查询
                    transaction.Commit();
                    return (List<SkillData>)list.List();
                }
            }
        }

        /// <summary>
        /// 获取技能
        /// </summary>
        /// <param name="_skillID"></param>
        /// <returns></returns>
        public List<SkillData> GetSkillBySkillID(int _skillID)
        {
            // 获取数据库Session
            using (var session = NHibernateHelper.OpenSession())
            {
                // 获取视图
                using (var transaction = session.BeginTransaction())
                {
                    var list = session.QueryOver<SkillData>().Where(x => x.SkillID == _skillID);
                    // 提交查询
                    transaction.Commit();
                    return (List<SkillData>)list.List();
                }
            }
        }

        /// <summary>
        /// 添加一个角色技能
        /// </summary>
        /// <param name="_rolfInfo"></param>
        public void AddSkill(SkillData _data)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(_data);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// 更新角色技能
        /// </summary>
        /// <param name="_rolfInfo"></param>
        public void UpdateSkill(SkillData _data)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Update(_data);
                    transaction.Commit();
                }
            }
        }
    }
}
