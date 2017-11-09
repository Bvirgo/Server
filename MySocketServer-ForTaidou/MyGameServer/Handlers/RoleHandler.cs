using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using MyGameServer.DataBase.Manager;
using MyGameCommon.Model;
using MyGameCommon;
using LitJson;
using ExitGames.Logging;
using MyGameServer;
using MyGameServer.Tool;
using MyGameServer.DataBase.Model;
using System.IO;
using MyGameServer.Plus;

namespace MyGameServer.Handlers
{
    public class RoleHandler:HandlerBase
    {
        private RoleMgr m_mgr;
        private UserLoginDataMgr m_userMgr;
        private ServerPropertyMgr m_serverMgr;

        RoleInfoMgr m_roleinfoMgr;
        GoodsListMgr m_goodsListMgr;
        EquipListMgr m_equiplistMgr;

        TaskDataMgr m_taskMgr;
        SkillDataMgr m_skillMgr;
        public RoleHandler()
        {
            m_mgr = new RoleMgr();
            m_userMgr = new UserLoginDataMgr();
            m_serverMgr = new ServerPropertyMgr();

            m_roleinfoMgr = new RoleInfoMgr();
            m_goodsListMgr = new GoodsListMgr();
            m_equiplistMgr = new EquipListMgr();

            m_taskMgr = new TaskDataMgr();
            m_skillMgr = new SkillDataMgr();
        }

        public override void OnHandlerMessage(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            byte oprCode = request.OperationCode;
            switch(oprCode)
            {
                    // 获取玩家角色列表
                case (byte)OperationCode.RoleHandler:
                    GetRoleList(request,response,peer,sendParameters);
                    break;
                    // 创建新角色
                case (byte)OperationCode.CreateRole:
                    CreateRole(request,response,peer,sendParameters);
                    break;
                case (byte)OperationCode.ChooseRole:
                    ChooseRole(request, response, peer, sendParameters);
                    break;
            }
          
        }

        void CreateRole(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Dictionary<byte, object> m_dic = request.Parameters;
            object value;
            response.ReturnCode = (short)ReturnCode.Fail;
            if (m_dic.TryGetValue((byte)ParameterCode.RoleInfo,out value))
            {
                //只需要传送：名称 + 性别 + 用户ID + 服务器ID
                Helper.Log("RoleHandler Create Role:"+value.ToString());
                string[] p = value.ToString().Split(',');
                if (p.Length == 4)
                {
                    Role newRole = new Role();
                    newRole.Name = p[0];
                    newRole.Lv = 1;
                    bool bMan = true;
                    string strOccup = "Man";
                    if (p[1].Equals("0"))
                    {
                        bMan = false;
                        strOccup = "Girl";
                    }
                    newRole.IsMan = bMan;
                    newRole.Occup = strOccup;

                    Helper.Log("当前服务器ID:"+p[3]);
                    int nServerId = Helper.IntParse(p[3]);
                    List<ServerProperty> pServer=  m_serverMgr.GetServerByID(Helper.IntParse(p[3]));
                    if (pServer == null || pServer.Count < 1)
                    {
                        Helper.Log("获取服务器失败！");
                        response.ReturnCode = (short)ReturnCode.Fail;
                        return;
                    }

                    newRole.Server = pServer[0];

                    List<User> pUser = m_userMgr.GetUserByID(Helper.IntParse(p[2]));
                    Helper.Log("当前用户ID:" + p[2]);
                    if (pUser == null || pUser.Count < 1)
                    {
                        Helper.Log("获取用户失败！");
                        response.ReturnCode = (short)ReturnCode.Fail;
                        return;
                    }

                    // 同一服务器一个名称只能存在一个角色
                    List<Role> pRoleData = m_mgr.GetServerRole(nServerId);
                    if (pRoleData != null)
                    {
                        for (int i = 0; i < pRoleData.Count;++i )
                        {
                            Role curRole = pRoleData[i];
                            if (curRole.Name == newRole.Name)
                            {
                                Helper.Log("获取用户失败！");
                                response.ReturnCode = (short)ReturnCode.Fail;
                                response.DebugMessage = "用户名已经存在，请重新输入！";
                                return;
                            }
                        }
                    }

                    newRole.User = pUser[0];
                    m_mgr.SaveRole(newRole);

                    // 为新角色创建：roleinfo,goodslist、task、skill表
                    RoleInfoData newInforData = new RoleInfoData();
                    newInforData.Role = newRole;
                    newInforData.Exp = 0;
                    newInforData.Gold = 100;
                    newInforData.Gem = 100;
                    newInforData.Energy = 80;
                    newInforData.Toughen = 40;
                    m_roleinfoMgr.AddRoleInfo(newInforData);

                    GoodsData newGoodsList = new GoodsData();
                    newGoodsList.Role = newRole;
                    newGoodsList.GoodsID = 1018;
                    newGoodsList.Num = 10;
                    newGoodsList.Lv = 1;
                    newGoodsList.Drs = 0;
                    m_goodsListMgr.AddGoodsList(newGoodsList);

                    // 任务表读XML配置把
                    string strBinaryPath = MyGameApplication.MyInstance.m_strBinaryPath;
                    string strTaskPath = (Path.Combine(strBinaryPath, "TaskConfig.xml"));
                    TaskConfigData taskdata = Helper.LoadXML<TaskConfigData>(strTaskPath);
                    List<TaskItem> m_pTask;
                    if (taskdata != null)
                    {
                        m_pTask = taskdata.m_task.m_pTaskList;
                        for (int i = 0; i < m_pTask.Count;++i )
                        {
                            TaskData newTask = new TaskData();
                            newTask.TaskID = m_pTask[i].ID;
                            newTask.TaskPro = 0;
                            newTask.TaskType = m_pTask[i].Type;
                            newTask.Role = newRole;
                            newTask.LastupdateTime = DateTime.Now.ToShortDateString();
                            m_taskMgr.AddTask(newTask);
                        }
                    }

                    // 技能配置
                    strTaskPath = (Path.Combine(strBinaryPath, "SkillConfig.xml"));
                    SkillConfigData data = Helper.LoadXML<SkillConfigData>(strTaskPath);
                    List<RoleSkill> pSkillData;
                    if (data != null)
                    {
                        pSkillData = data.m_pSkillList;
                        RoleSkill skill = null;
                        for (int i = 0; i < pSkillData.Count; ++i)
                        {
                            if (pSkillData[i].Sex == 1 && newRole.IsMan)
                            {
                                skill = pSkillData[i];
                                break;
                            }
                            else if (pSkillData[i].Sex == 0 && !newRole.IsMan)
                            {
                                skill = pSkillData[i];
                                break;
                            }
                        }
                        List<int> m_pSkillID = new List<int>();
                        m_pSkillID.Add(skill.SkillBase);
                        m_pSkillID.Add(skill.SkillOne);
                        m_pSkillID.Add(skill.SkillTwo);
                        m_pSkillID.Add(skill.SkillThere);
                        for (int i = 0;i < 4;++i)
                        {
                            SkillData newSkill = new SkillData();
                            newSkill.Lv = 1;
                            newSkill.Pos = i;

                            newSkill.SkillID = m_pSkillID[i];
                            newSkill.Role = newRole;
                            m_skillMgr.AddSkill(newSkill);
                        }
                    }

                    // 保存当前选择的角色
                    List<Role> pChooseRole = m_mgr.GetRoleByName(newRole.Name, newRole.Server.ID);
                    if (pChooseRole != null && pChooseRole.Count == 1)
                    {
                        peer.m_curRole = pChooseRole[0];
                        Helper.Log("当前选择角色："+peer.m_curRole.Name);
                    }

                    // 返回角色列表
                    List<Role> pData = m_mgr.GetUserRole(newRole.User.Id, newRole.Server.ID);
                    // 获取角色列表时候，第一个角色就是默认角色
                    string strRes = "";
                    for (int i = 0; i < pData.Count; ++i)
                    {
                        Role curRole = pData[i];
                        strRes += curRole.Id + "," + curRole.Name + "," + curRole.Lv + "," + curRole.IsMan + "," + curRole.Occup + "," + curRole.User.Id + "," + curRole.Server.ID + "|";
                    }
                    Dictionary<byte, object> dic = new Dictionary<byte, object>();
                    dic.Add((byte)ParameterCode.RoleInfo, strRes);
                    response.Parameters = dic;

                    response.ReturnCode = (short)ReturnCode.Success;
                    Helper.Log("RoleHandler Create Role Success");
                    return;
                }
            }
            response.ReturnCode = (short)ReturnCode.Fail;
            return;
        }

        void GetRoleList(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Dictionary<byte, object> m_dic = request.Parameters;
            // 获取客户端发送过来的用户+服务器验证数据
            object value;
            response.ReturnCode = (short)ReturnCode.Fail;
            if (m_dic.TryGetValue((byte)ParameterCode.RoleInfo, out value))
            {
                Helper.Log("RoleHanler Get Value:" + value.ToString());
                string[] p = value.ToString().Split(',');
                if (p.Length == 2)
                {
                    int nUserID = Helper.IntParse(p[0]);
                    int nServerID = Helper.IntParse(p[1]);
                    List<Role> pData = m_mgr.GetUserRole(nUserID, nServerID);
                    if (pData != null && pData.Count > 0)
                    {
                        // 获取角色列表时候，第一个角色就是默认角色
                        peer.m_curRole = pData[0];
                        string strRes = "";
                        for (int i = 0; i < pData.Count; ++i)
                        {
                            Role curRole = pData[i];
                            strRes += strRes + curRole.Id + "," + curRole.Name + "," + curRole.Lv + "," + curRole.IsMan + "," + curRole.Occup + "," + curRole.User.Id + "," + curRole.Server.ID + "|";
                        }
                        Dictionary<byte, object> dic = new Dictionary<byte, object>();
                        dic.Add((byte)ParameterCode.RoleInfo, strRes);
                        response.ReturnCode = (short)ReturnCode.Success;
                        response.Parameters = dic;
                        return;
                    }
                }
                response.DebugMessage = "解析用户ID 和服务器ID失败!";
                response.ReturnCode = (short)ReturnCode.Fail;
                return;
            }
            response.DebugMessage = "没有收到客户端发送的用户ID和服务器ID数据";
            response.ReturnCode = (short)ReturnCode.Fail;
            return;
        }

        void ChooseRole(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Dictionary<byte, object> m_dic = request.Parameters;
            // 获取客户端发送过来的角色名+服务器ID
            object value;
            response.ReturnCode = (short)ReturnCode.Fail;
            if (m_dic.TryGetValue((byte)ParameterCode.RoleInfo, out value))
            {
                Helper.Log("Choose Role:"+value.ToString());
                string[] p = value.ToString().Split(',');
                if (p.Length == 2)
                {
                    string nRoleName = p[0];
                    int nServerID = Helper.IntParse(p[1]);
                    List<Role> pRole = m_mgr.GetRoleByName(nRoleName, nServerID);
                    if (pRole != null && pRole.Count > 0)
                    {
                        peer.m_curRole = pRole[0];
                        response.ReturnCode = (short)ReturnCode.Success;
                        Helper.Log("Choose Success");
                        return;
                    }
                }
            }
        }
    }
}
