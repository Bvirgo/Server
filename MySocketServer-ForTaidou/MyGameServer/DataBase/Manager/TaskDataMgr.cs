using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGameCommon.Model;
using MyGameServer.DataBase.Model;

namespace MyGameServer.DataBase.Manager
{
    public class TaskDataMgr
    {
        /// <summary>
        /// 获取角色下面的任务列表
        /// </summary>
        /// <param name="_roleID"></param>
        /// <returns></returns>
        public List<TaskData> GetTaskList(int _roleID)
        {
            // 获取数据库Session
            using (var session = NHibernateHelper.OpenSession())
            {
                // 获取视图
                using (var transaction = session.BeginTransaction())
                {
                    var taskList = session.QueryOver<TaskData>().Where(x => x.Role.Id == _roleID);
                    // 提交查询
                    transaction.Commit();
                    return (List<TaskData>)taskList.List();
                }
            }
        }


        /// <summary>
        /// 根据任务ID，获取对应的任务信息
        /// </summary>
        /// <param name="_taskID"></param>
        /// <returns></returns>
        public List<TaskData> GetTaskByTaskID(int _taskID)
        {
            // 获取数据库Session
            using (var session = NHibernateHelper.OpenSession())
            {
                // 获取视图
                using (var transaction = session.BeginTransaction())
                {
                    var taskList = session.QueryOver<TaskData>().Where(x => x.TaskID == _taskID);
                    // 提交查询
                    transaction.Commit();
                    return (List<TaskData>)taskList.List();
                }
            }
        }

        /// <summary>
        /// 添加一个任务
        /// </summary>
        /// <param name="_task"></param>
        public void AddTask(TaskData _task)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    // 获取服务器当前时间作为最后更新时间
                    _task.LastupdateTime = DateTime.Now.ToShortDateString();
                    session.Save(_task);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// 更新任务信息
        /// </summary>
        /// <param name="_task"></param>
        public void UpdateTask(TaskData _task)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    // 获取服务器当前时间作为最后更新时间
                    session.Update(_task);
                    transaction.Commit();
                }
            }
        }
    }
}
