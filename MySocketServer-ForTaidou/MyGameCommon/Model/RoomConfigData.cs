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
    public class RoomConfigData
    {
        public RoomConfigData() { }
        [XmlElementAttribute("Room")]
        public List<RoomInfo> m_pRoom = new List<RoomInfo>();
    }

    [XmlRootAttribute("Room")]
    public class RoomInfo
    {
        public RoomInfo() { }
        [XmlAttribute("id")]
        public int m_nID { get; set; }
        [XmlElementAttribute("Trigger")]
        public List<TriggerInfo> m_pTrigger = new List<TriggerInfo>();

        [XmlElementAttribute("Spawn")]
        public List<RoleSpawn> m_pSpawn = new List<RoleSpawn>();
    }

    [XmlRootAttribute("Spawn")]
    public class RoleSpawn
    {
        public RoleSpawn() { }
        [XmlAttribute("x")]
        public float m_fX { get; set; }
        [XmlAttribute("y")]
        public float m_fY { get; set; }

        [XmlAttribute("z")]
        public float m_fZ { get; set; }
    }
    [XmlRootAttribute("Trigger")]
    public class TriggerInfo
    {
        public TriggerInfo() { }
        [XmlAttribute("id")]
        public int m_nID { get; set; }
        [XmlAttribute("modelid")]
        public string m_strModelID { get; set; }
        [XmlAttribute("num")]
        public int m_nNum { get; set; }
        [XmlElementAttribute("Pos")]
        public List<TriggerPosInfo> m_pPosList = new List<TriggerPosInfo>();
    }

    [XmlRootAttribute("Pos")]
    public class TriggerPosInfo
    {
        public TriggerPosInfo(){}

        [XmlAttribute("x")]
        public float m_fX { get; set; }
        [XmlAttribute("y")]
        public float m_fY { get; set; }

        [XmlAttribute("z")]
        public float m_fZ { get; set; }
    }
}
