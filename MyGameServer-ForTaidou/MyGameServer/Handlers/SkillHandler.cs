using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using MyGameServer.DataBase.Manager;
using MyGameCommon.Model;
using MyGameCommon;
using ExitGames.Logging;
using MyGameServer;
using MyGameServer.Tool;
using MyGameServer.DataBase.Model;

namespace MyGameServer.Handlers
{
    public class SkillHandler:HandlerBase
    {
        private SkillDataMgr m_skillMgr;
        private RoleInfoMgr m_roleMgr;
        public SkillHandler()
        {
            m_skillMgr = new SkillDataMgr();
            m_roleMgr = new RoleInfoMgr();
        }
        public override void OnHandlerMessage(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            byte operCode = request.OperationCode;
            switch(operCode)
            {
                case (byte)OperationCode.GetSkill:
                    GetSkillList(request,response,peer,sendParameters);
                    break;
                case (byte)OperationCode.AddSkill:
                    AddSkill(request,response,peer,sendParameters);
                    break;
                case (byte)OperationCode.UpdateSkill:
                    UpdateSkill(request,response,peer,sendParameters);
                    break;
            }
        }

        /// <summary>
        /// 获取玩家技能
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="peer"></param>
        /// <param name="sendParameters"></param>
        void GetSkillList(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Helper.Log("SkillHandler GetSkillInfo");
            response.ReturnCode = (short)ReturnCode.Fail;
            List<SkillData> pList = m_skillMgr.GetSkillData(peer.m_curRole.Id);
            if (pList != null && pList.Count > 0)
            {
                string str = "";
                for (int i = 0; i < pList.Count; ++i)
                {
                    SkillData data = pList[i];
                    // skillid,lv,pos
                    str += data.SkillID + "," + data.Lv + ","+data.Pos+"|";
                }
                Dictionary<byte, object> dic = new Dictionary<byte, object>();
                dic.Add((byte)ParameterCode.SkillInfo, str);
                response.Parameters = dic;
                response.ReturnCode = (short)ReturnCode.Success;
                Helper.Log("GetSkillList Success:" + str);
            }
            return;
        }


        /// <summary>
        /// 添加技能
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="peer"></param>
        /// <param name="sendParameters"></param>
        void AddSkill(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Dictionary<byte, object> m_dic = request.Parameters;
            object value;
            response.ReturnCode = (short)ReturnCode.Fail;
            // 客户端传递过来的参数信息：skillid,pos
            if (m_dic.TryGetValue((byte)ParameterCode.SkillInfo, out value))
            {
                string strValue = value.ToString();
                Helper.Log("AddSkill:" + strValue);
                string[] pStr1 = strValue.Split(',');
                if (pStr1.Length == 2)
                {
                    SkillData data = new SkillData();
                    data.SkillID = Helper.IntParse(pStr1[0]);
                    data.Lv = Helper.IntParse(pStr1[1]);
                    data.Pos = 1;
                    data.Role = peer.m_curRole;
                    m_skillMgr.AddSkill(data);
                    Helper.Log("Add Skill Success:" + data.SkillID);
                    response.ReturnCode = (short)ReturnCode.Success;
                }
            }
            return;
        }

        /// <summary>
        /// 技能升级
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="peer"></param>
        /// <param name="sendParameters"></param>
        void UpdateSkill(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Dictionary<byte, object> m_dic = request.Parameters;
            object value;
            response.ReturnCode = (short)ReturnCode.Fail;
            // 客户端传递过来的参数信息：skillid,lv
            if (m_dic.TryGetValue((byte)ParameterCode.SkillInfo, out value))
            {
                string strValue = value.ToString();
                Helper.Log("UpdateSkill:" + strValue);
                string[] pStr1 = strValue.Split(',');
                if (pStr1.Length == 2)
                {
                    int nSkillID = Helper.IntParse(pStr1[0]);
                    List<SkillData> pList = m_skillMgr.GetSkillBySkillID(nSkillID);
                    if (pList != null && pList.Count >0)
                    {
                        SkillData data = pList[0];
                        data.Lv = Helper.IntParse(pStr1[1]);
                        // 更新角色金币
                        int nNeedGold = data.Lv * 100;
                        RoleData curRole = peer.m_curRole;
                        List<RoleInfoData> p = m_roleMgr.GetRoleData(curRole.Id);
                        if (p != null && p.Count > 0)
                        {
                            RoleInfoData curRoleData = p[0];
                            if (curRoleData.Gold >= nNeedGold && curRole.Lv > data.Lv)
                            {
                                // 客户端金币减少的时候会同步一次，这里不就用同步了
                                //m_roleMgr.UpdateRoleInfo(curRoleData);
                                data.Lv += 1;
                                m_skillMgr.UpdateSkill(data);

                                Helper.Log("UpdateSkill Success:" + data.SkillID);
                                response.ReturnCode = (short)ReturnCode.Success;
                            }
                        }
                    }
                }
            }
            return;
        }
    }
}
