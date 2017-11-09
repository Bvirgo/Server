using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameServer.DataBase.Model
{
    public class TaskData
    {
        public virtual int ID { get; set; }
        public virtual Role Role { get; set; }// 任务所属角色
        public virtual int TaskID { get; set; }// 任务ID
        public virtual int TaskType { get; set; }// 任务类型:
        public virtual int TaskPro { get; set; }// 任务进度

        public virtual string LastupdateTime { get; set; }// 最后更新时间
    }

    public enum TaskPro
    {
        Nostart = 0,//还没接受
        Accept = 1,// 已经接受(正在进行)
        Complete = 2,// 已经完成
        Got = 3// 已经领取奖励
    }

    public enum TaskType
    {
        Main = 0,// 主线
        Reward = 1,// 成就任务
        Daily = 2// 日常任务
    }
}
