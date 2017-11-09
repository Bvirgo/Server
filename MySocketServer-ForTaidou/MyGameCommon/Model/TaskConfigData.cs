using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MyGameServer.Plus
{
    [XmlRootAttribute("Root")]
    public class TaskConfigData
    {
        public TaskConfigData() { }
        [XmlElementAttribute("Lv")]
        public RoleTask m_task = new RoleTask();
    }

    [XmlRootAttribute("Lv")]
    public class RoleTask
    {
        public RoleTask() { }
        [XmlElementAttribute("Task")]
        public List<TaskItem> m_pTaskList = new List<TaskItem>();
    }

    [XmlRootAttribute("Task")]
    public class TaskItem
    {
        public TaskItem() { }
        [XmlAttribute("id")]
        public int ID
        {
            get;
            set;
        }
        [XmlAttribute("type")]
        public int Type
        {
            get;
            set;
        }
    }
}
