using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using MyGameServer.Tool;

namespace MyGameServer.Plus
{
    [XmlRootAttribute("Root")]
    public class MonsterConfigData
    {
        public MonsterConfigData() { }
        [XmlElementAttribute("Monster")]
        public List<MonsterInfo> m_pMonster = new List<MonsterInfo>();
    }
    [XmlRootAttribute("Monster")]
    public class MonsterInfo
    {
        public MonsterInfo() { }
        [XmlAttribute("id")]
        public string m_strModelId { get; set; }
        [XmlAttribute("hp")]
        public int m_nHp { get; set; }
        [XmlAttribute("attackDis")]
        public float m_fAttackDis { get; set; }
        [XmlAttribute("atkCD")]
        public int m_nAtkCD { get; set; }
        [XmlAttribute("damage")]
        public int m_nDamage { get; set; }
        [XmlAttribute("cruise")]
        public int m_nCruiseSpeed { get; set; }
        [XmlAttribute("pursuit")]
        public int m_nPursuitSpeed { get; set; }
        public string m_strGuid;
    }

    public class Monster
    {
        public Monster()
        {

        }

        public Monster(MonsterInfo curM)
        {
            m_nAtkCD = curM.m_nAtkCD;
            m_fAttackDis = curM.m_fAttackDis;
            m_nCruiseSpeed = curM.m_nCruiseSpeed;
            m_nDamage = curM.m_nDamage;
            m_nHp = curM.m_nHp;
            m_nPursuitSpeed = curM.m_nPursuitSpeed;
            m_strModelId = curM.m_strModelId;
            m_strGuid = curM.m_strGuid;
        }
        // 模型ID
        public string m_strModelId { get; set; }
        // 血量
        public int m_nHp { get; set; }
        //攻击距离
        public float m_fAttackDis { get; set; }
        // CD
        public int m_nAtkCD { get; set; }
        public int m_nDamage { get; set; }
        public int m_nCruiseSpeed { get; set; }
        public int m_nPursuitSpeed { get; set; }
        public string m_strGuid;
        // 目的点坐标
        public Vector3 m_targetPos;
        // 当前位置
        public Vector3 m_curPos;

        // Monster动作状态：1：站立 2 ：巡航 3：追击 4：攻击 
        public byte m_ActionType;
    }
}
