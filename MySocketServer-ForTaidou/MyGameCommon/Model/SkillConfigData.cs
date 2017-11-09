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
    public class SkillConfigData
    {
        public SkillConfigData() { }
        [XmlElementAttribute("Role")]
        public List<RoleSkill> m_pSkillList = new List<RoleSkill>();
    }

    [XmlRootAttribute("Role")]
    public class RoleSkill
    {
        public RoleSkill(){}
        [XmlAttribute("Sex")]
        public int Sex{get;set;}
        [XmlAttribute("SkillBase")]
        public int SkillBase{get;set;}
        [XmlAttribute("SkillOne")]
        public int SkillOne{get;set;}
        [XmlAttribute("SkillTwo")]
        public int SkillTwo{get;set;}
        [XmlAttribute("SkillThere")]
        public int SkillThere{get;set;}
    }
}
