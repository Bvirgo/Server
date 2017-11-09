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
    public class RoleInfoHandler : HandlerBase
    {
        RoleInfoMgr m_roleinfoMgr;
        GoodsListMgr m_goodsListMgr;
        EquipListMgr m_equiplistMgr;

        RoleMgr m_roleMgr;

        public RoleInfoHandler()
        {
            m_roleinfoMgr = new RoleInfoMgr();
            m_goodsListMgr = new GoodsListMgr();
            m_equiplistMgr = new EquipListMgr();

            m_roleMgr = new RoleMgr();
        }

        public override void OnHandlerMessage(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            byte operCode = request.OperationCode;
            switch (operCode)
            {
                case (byte)OperationCode.UpdateRoleInfo:
                    UpdateRoleInfo(request, response, peer, sendParameters);
                    break;
                case (byte)OperationCode.GetRoleInfo:
                    GetRoleInfo(request, response, peer, sendParameters);
                    break;
                case (byte)OperationCode.AddGoodsList:
                    AddGoodsInfo(request, response, peer, sendParameters);
                    break;
                case (byte)OperationCode.UpdateGoodsList:
                    UpdateGoodsInfo(request, response, peer, sendParameters);
                    break;
                case (byte)OperationCode.GetGoodsList:
                    GetGoodsInfo(request, response, peer, sendParameters);
                    break;
                case (byte)OperationCode.AddEquipList:
                    AddEuipInfo(request, response, peer, sendParameters);
                    break;
                case (byte)OperationCode.UpdateEquipList:
                    UpdateEquipInfo(request, response, peer, sendParameters);
                    break;
                case (byte)OperationCode.GetEquipList:
                    GetEquipInfo(request, response, peer, sendParameters);
                    break;
            }
        }

        /// <summary>
        /// 获取角色基础属性
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="peer"></param>
        /// <param name="sendParameters"></param>
        void GetRoleInfo(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Helper.Log("RoleInfoHandler GetRoleInfo");
            response.ReturnCode = (short)ReturnCode.Fail;
            List<RoleInfoData> pRoleInfo = m_roleinfoMgr.GetRoleData(peer.m_curRole.Id);
            if (pRoleInfo != null && pRoleInfo.Count > 0)
            {
                RoleInfoData roleInfo = pRoleInfo[0];
                // exp,gold,gem,energy,toughen
                string str = "";
                str += roleInfo.Exp + "," + roleInfo.Gold + "," + roleInfo.Gem + "," + roleInfo.Energy + "," + roleInfo.Toughen;
                Dictionary<byte, object> dic = new Dictionary<byte, object>();
                dic.Add((byte)ParameterCode.RoleData, str);
                response.Parameters = dic;
                response.ReturnCode = (short)ReturnCode.Success;
                Helper.Log("GetRoleInfo Success:"+str);
            }
            return;
        }
        /// <summary>
        /// 更新角色基础属性
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="peer"></param>
        /// <param name="sendParameters"></param>
        void UpdateRoleInfo(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Dictionary<byte, object> m_dic = request.Parameters;
            object value;
            response.ReturnCode = (short)ReturnCode.Fail;
            // 客户端传递过来的参数信息：exp,gold,gem,energy,toughen,name,lv
            if (m_dic.TryGetValue((byte)ParameterCode.RoleData, out value))
            {
                string strValue = value.ToString();
                Helper.Log("UpdateRoleInfo:" + strValue);
                string[] pStr1 = strValue.Split(',');
                if (pStr1.Length == 7)
                {
                    List<RoleInfoData> pList = m_roleinfoMgr.GetRoleData(peer.m_curRole.Id);
                    if (pList != null && pList.Count > 0)
                    {
                        RoleInfoData data = pList[0];
                        data.Exp = Helper.IntParse(pStr1[0]);
                        data.Gold = Helper.IntParse(pStr1[1]);
                        data.Gem = Helper.IntParse(pStr1[2]);
                        data.Energy = Helper.IntParse(pStr1[3]);
                        data.Toughen = Helper.IntParse(pStr1[4]);
                        m_roleinfoMgr.UpdateRoleInfo(data);

                        // 更新角色名称和等级
                        RoleData curRole = peer.m_curRole;
                        curRole.Name = pStr1[5];
                        curRole.Lv = Helper.IntParse(pStr1[6]);

                        m_roleMgr.UpdateUser(curRole);

                        Helper.Log("Update RoleInfo:" + data);
                        response.ReturnCode = (short)ReturnCode.Success;
                    }
                }
            }
            return;
        }

        /// <summary>
        /// 获取角色物品列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="peer"></param>
        /// <param name="sendParameters"></param>
        void GetGoodsInfo(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Helper.Log("RoleInfoHandler GetGoodsList");
            response.ReturnCode = (short)ReturnCode.Fail;
            List<GoodsData> pGoodsList = m_goodsListMgr.GetGoodsList(peer.m_curRole.Id);
            if (pGoodsList != null && pGoodsList.Count > 0)
            {
                string str = "";
                for (int i = 0; i < pGoodsList.Count; ++i)
                {
                    GoodsData goods = pGoodsList[i];
                    // id,goodsid,num,lv,drs
                    str += goods.ID + "," + goods.GoodsID + "," + goods.Num + "," + goods.Lv + ","+goods.Drs+"|";
                }
                Dictionary<byte, object> dic = new Dictionary<byte, object>();
                dic.Add((byte)ParameterCode.RoleData, str);
                response.Parameters = dic;
                response.ReturnCode = (short)ReturnCode.Success;
                Helper.Log("GetGoodsInfo Success:" + str);
            }
            return;
        }
        /// <summary>
        /// 角色获取物品
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="peer"></param>
        /// <param name="sendParameters"></param>
        void AddGoodsInfo(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Dictionary<byte, object> m_dic = request.Parameters;
            object value;
            response.ReturnCode = (short)ReturnCode.Fail;
            // 客户端传递过来的参数信息：goodsid,num,lv,drs
            if (m_dic.TryGetValue((byte)ParameterCode.RoleData, out value))
            {
                string strValue = value.ToString();
                Helper.Log("UpdateGoodsInfo:" + strValue);
                string[] pStr1 = strValue.Split(',');
                if (pStr1.Length == 4)
                {
                    GoodsData data = new GoodsData();
                    data.GoodsID = Helper.IntParse(pStr1[0]);
                    data.Num = Helper.IntParse(pStr1[1]);
                    data.Lv = Helper.IntParse(pStr1[2]);
                    data.Drs = byte.Parse(pStr1[3]);
                    int nId = m_goodsListMgr.AddGoodsList(data);

                    List<GoodsData> pNewGoods = m_goodsListMgr.GetGoodsByID(nId);
                    if (pNewGoods != null && pNewGoods.Count > 0)
                    {
                        data = pNewGoods[0];
                    }
                    Helper.Log("Add GoodsInfo:" + data.ID);

                    // 返回新物品
                    string str = data.ID + "," + data.GoodsID + "," + data.Num + "," + data.Lv + "," + data.Drs;
                    Dictionary<byte, object> m_dicGoods = new Dictionary<byte, object>();
                    m_dicGoods.Add((byte)ParameterCode.RoleData, str);

                    response.Parameters = m_dic;
                    response.ReturnCode = (short)ReturnCode.Success;
                }
            }
            return;
        }
        /// <summary>
        /// 更新角色物品信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="peer"></param>
        /// <param name="sendParameters"></param>
        void UpdateGoodsInfo(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Dictionary<byte, object> m_dic = request.Parameters;
            object value;
            response.ReturnCode = (short)ReturnCode.Fail;
            // 客户端传递过来的参数信息：id,num,lv,drs
            if (m_dic.TryGetValue((byte)ParameterCode.RoleData, out value))
            {
                string strValue = value.ToString();
                Helper.Log("UpdateGoodsInfo:" + strValue);
                string[] pStr1 = strValue.Split(',');
                if (pStr1.Length == 4)
                {
                    int nId = Helper.IntParse(pStr1[0]);
                    List<GoodsData> pList = m_goodsListMgr.GetGoodsByID(nId);
                    if (pList != null && pList.Count > 0)
                    {
                        GoodsData data = pList[0];
                        data.Num = Helper.IntParse(pStr1[1]);
                        data.Lv = Helper.IntParse(pStr1[2]);
                        data.Drs = byte.Parse(pStr1[3]);
                        if (data.Num > 0)
                        {
                            m_goodsListMgr.UpdateGoodsList(data);
                        }
                        else
                        {
                            m_goodsListMgr.DeleteGoods(data);
                        }

                        Helper.Log("Update GoodsInfo:" + data);
                        response.ReturnCode = (short)ReturnCode.Success;
                    }
                }
            }
            return;
        }
        /// <summary>
        /// 获取装备信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="peer"></param>
        /// <param name="sendParameters"></param>
        void GetEquipInfo(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Helper.Log("RoleInfoHandler GetEquipList");
            response.ReturnCode = (short)ReturnCode.Fail;
            List<EquipData> pList = m_equiplistMgr.GetEquipList(peer.m_curRole.Id);
            if (pList != null && pList.Count > 0)
            {
                string str = "";
                for (int i = 0; i < pList.Count; ++i)
                {
                    EquipData data = pList[i];
                    // goodsid,lv
                    str += data.GoodsID + "," + data.Lv + "|";
                }
                Dictionary<byte, object> dic = new Dictionary<byte, object>();
                dic.Add((byte)ParameterCode.RoleData, str);
                response.Parameters = dic;
                response.ReturnCode = (short)ReturnCode.Success;
                Helper.Log("GetEquipInfo Success:" + str);
            }
            return;
        }
        /// <summary>
        /// 添加新装备
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="peer"></param>
        /// <param name="sendParameters"></param>
        void AddEuipInfo(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Dictionary<byte, object> m_dic = request.Parameters;
            object value;
            response.ReturnCode = (short)ReturnCode.Fail;
            // 客户端传递过来的参数信息：goodsid,lv
            if (m_dic.TryGetValue((byte)ParameterCode.RoleData, out value))
            {
                string strValue = value.ToString();
                Helper.Log("UpdateGoodsInfo:" + strValue);
                string[] pStr1 = strValue.Split(',');
                if (pStr1.Length == 2)
                {
                    EquipData data = new EquipData();
                    data.GoodsID = Helper.IntParse(pStr1[0]);
                    data.Lv = Helper.IntParse(pStr1[1]);
                    m_equiplistMgr.AddEquipList(data);
                    Helper.Log("Add EquipInfo:" + data);
                    response.ReturnCode = (short)ReturnCode.Success;
                }
            }
            return;
        }
        /// <summary>
        /// 更新角色装备信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="peer"></param>
        /// <param name="sendParameters"></param>
        void UpdateEquipInfo(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Dictionary<byte, object> m_dic = request.Parameters;
            object value;
            response.ReturnCode = (short)ReturnCode.Fail;
            // 客户端传递过来的参数信息：goodsid,lv
            if (m_dic.TryGetValue((byte)ParameterCode.RoleData, out value))
            {
                string strValue = value.ToString();
                Helper.Log("UpdateEquipList:" + strValue);
                string[] pStr1 = strValue.Split(',');
                if (pStr1.Length == 2)
                {
                    int nId = Helper.IntParse(pStr1[0]);
                    List<EquipData> pList = m_equiplistMgr.GetEquipDataByID(nId);
                    if (pList != null && pList.Count > 0)
                    {
                        EquipData data = pList[0];
                        data.Lv = Helper.IntParse(pStr1[1]);
                        m_equiplistMgr.UpdateEquipList(data);
                        Helper.Log("Update GoodsInfo:" + data);
                        response.ReturnCode = (short)ReturnCode.Success;
                    }
                }
            }
            return;
        }
    }
}
