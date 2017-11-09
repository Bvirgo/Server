using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using MyGameCommon.Model;
using MyGameServer.DataBase.Model;

namespace MyGameServer.DataBase.Map
{
    public class TaskDataMap:ClassMap<TaskData>
    {
        public TaskDataMap()
        {
            Id(x => x.ID).Column("id");
            Map(x => x.TaskID).Column("taskid");
            Map(x => x.TaskType).Column("tasktype");
            Map(x=>x.TaskPro).Column("taskpro");
            Map(x => x.LastupdateTime).Column("lastupdatetime");
            References(x => x.Role).Column("roleid");

            Table("task");
        }
    }
}
