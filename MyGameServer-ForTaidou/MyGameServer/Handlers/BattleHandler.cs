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
using MyGameServer.Plus;

namespace MyGameServer.Handlers
{
    public class BattleHandler:HandlerBase
    {
        // 所属队伍
        public BattleTeam m_Team;

        public override void OnHandlerMessage(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            byte operCode = request.OperationCode;
            switch(operCode)
            {
                case (byte)OperationCode.ForTeam: // 组队申请:最多3人一个队伍，也可以两人
                    OnBattleTeam(request, response, peer, sendParameters);
                    break;
                case (byte)OperationCode.CancelTeam: // 取消组队:从系统组队列表中移除当前的Peer
                    MyGameApplication.MyInstance.RemovePeer(peer.m_strRoomID, peer);
                    response.ReturnCode = (short)ReturnCode.Success;
                    break;
                case (byte)OperationCode.SyncMove:
                    SyncRoleMove(request, response, peer, sendParameters);
                    break;
                case (byte)OperationCode.SyncMoveAnim:
                    SyncRoleMoveAnim(request,response,peer,sendParameters);
                    break;
                case (byte)OperationCode.SyncMoveDir:
                    SyncRoleMoveDir(request, response,peer,sendParameters);
                    break;
            }
        }

        /// <summary>
        /// 申请组队，返回队伍信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="peer"></param>
        /// <param name="sendParameters"></param>
        void OnBattleTeam(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Dictionary<byte, object> dicPara = request.Parameters;
            object value;
            if (dicPara.TryGetValue((byte)ParameterCode.BattleInfo, out value))
            {
                // 申请进入的副本ID
                peer.m_strRoomID = value.ToString();
            }

            if (DataManager.Instance != null)
            {
                // 获取副本中角色出生点位置
                peer.m_pRoleSpawn = DataManager.Instance.GetRoomSpawn(peer.m_strRoomID);
            }

            List<ClientPeer> pAllPeer = MyGameApplication.MyInstance.GetTeam(peer.m_strRoomID);

            if (pAllPeer != null)
            {
                // 满足队伍请求:创建一个Team,把队伍中角色信息返回给客户端
                int nLenth = pAllPeer.Count;
                // 队伍中只能取两个
                if (nLenth > 2)
                {
                    nLenth = 2;
                }
                List<ClientPeer> pTeam = new List<ClientPeer>();
                for (int i = 0; i < nLenth; ++i)
                {
                    pTeam.Add(pAllPeer[i]);
                    // 组队成功，移除Peer
                    MyGameApplication.MyInstance.RemovePeer(pAllPeer[i].m_strRoomID,pAllPeer[i]);
                }

                // 把当前的Peer加入进去
                pTeam.Add(peer);

                m_Team = new BattleTeam(pTeam);
                //排序
                pTeam.Sort(SortTeamRole);
                // 返回队伍中角色信息:所有peer都应该返回角色信息
                // 当前Peer返回给对应客户端角色信息
                string strRes = "";
                for (int j = 0; j < pTeam.Count; ++j)
                {
                    RoleData curRole = pTeam[j].m_curRole;
                    strRes += curRole.Id + "," + curRole.Name + "," + curRole.Lv + "," + curRole.IsMan + "," + curRole.Occup + "," + curRole.User.Id + "," + curRole.Server.ID + "," + (j + 1).ToString() + "|";

                    // 初始化角色出生点位置
                    if (peer.m_pRoleSpawn != null && 3 == peer.m_pRoleSpawn.Count)
                    {
                        RoleSpawn curSpawn = peer.m_pRoleSpawn[j];
                        Vector3 curPos = new Vector3(curSpawn.m_fX, curSpawn.m_fY, curSpawn.m_fZ);
                        pTeam[j].m_vCurPos = curPos;
                    }
                }
                Dictionary<byte, object> dic = new Dictionary<byte, object>();
                dic.Add((byte)ParameterCode.BattleInfo, strRes);
                response.ReturnCode = (short)ReturnCode.GotTeam;
                response.Parameters = dic;

                // 通知队伍中的其他Peer返回队伍角色信息给客户端
                for (int k = 0; k < pTeam.Count; ++k)
                {
                    ClientPeer curPeer = pTeam[k];
                    if (curPeer != peer)
                    {
                        // 不能用这个方法：这个方法只有是在客户端往服务器发送请求的时候一对一的匹配返回，不能服务器自动发起
                        //curPeer.SendOperationResponse(response, sendParameters);

                        // 可以用EventData代替,这个方法回调客户端的：OnEvent()方法
                        EventData eventData = new EventData();
                        eventData.Parameters = dic;
                        // 返回操作码
                        eventData.Code = (byte)OperationCode.ForTeam;
                        curPeer.SendEvent(eventData, new SendParameters());
                        Helper.Log(curPeer.m_curRole.Name + ":返回队伍信息：" + strRes);
                    }
                }
            }
            else // 找不到组队队友，等待
            {
                // 加入副本对应的队伍申请列表
                MyGameApplication.MyInstance.AddTeam(peer.m_strRoomID, peer);
                response.ReturnCode = (short)ReturnCode.Waitting;
                Helper.Log(peer.m_curRole.Name + ":等待队伍信息......");
            }
        }

        /// <summary>
        /// 队伍角色，根据ID排序
        /// </summary>
        int SortTeamRole(ClientPeer _a, ClientPeer _b)
        {
            return _a.m_curRole.Id > _b.m_curRole.Id ? -1 : 1;
        }
        /// <summary>
        /// 同步角色移动位置
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="peer"></param>
        /// <param name="sendParameters"></param>
        void SyncRoleMove(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Dictionary<byte, object> m_dic = request.Parameters;
            object value;

            // 客户端传递过来的参数信息：角色ID，坐标：X,Y,Z  Y轴旋转:_rY
            if (m_dic.TryGetValue((byte)ParameterCode.SyncInfo, out value))
            {
                string strValue = value.ToString();
                //Helper.Log("SyncRoleMove:" + strValue);
                string[] pStr1 = strValue.Split(',');
                if (pStr1.Length == 5)
                {
                    // 保存角色位置信息
                    float x = float.Parse(pStr1[1]);
                    float y = float.Parse(pStr1[2]);
                    float z = float.Parse(pStr1[3]);
                    peer.m_vCurPos.x = x;
                    peer.m_vCurPos.y = y;
                    peer.m_vCurPos.z = z;

                    string strRes = pStr1[0];
                    for (int i = 1; i < 5;++i )
                    {
                        strRes += ","+ pStr1[i];
                    }
                    m_dic.Clear();
                    m_dic.Add((byte)ParameterCode.SyncInfo,strRes);

                    if (null == peer.m_myTeam)
                    {
                        return;
                    }

                    List<ClientPeer> pTeam = peer.m_myTeam.m_pTeam;
                    if (pTeam == null)
                    {
                        return;
                    }

                    // 通知队伍中的其他Peer返回队伍角色信息给客户端
                    for (int k = 0; k < pTeam.Count; ++k)
                    {
                        ClientPeer curPeer = pTeam[k];

                        // 九宫格同步
                        bool bSync = Helper.IsSudoKu(peer.m_vCurPos, curPeer.m_vCurPos);

                        if (curPeer != peer && bSync)
                        {
                            EventData eventData = new EventData();
                            eventData.Parameters = m_dic;
                            // 返回操作码
                            eventData.Code = (byte)OperationCode.SyncMove;
                            curPeer.SendEvent(eventData, new SendParameters());
                            float dis = Vector3.Distance(peer.m_vCurPos, curPeer.m_vCurPos);
                            Helper.Log("角色1位置："+peer.m_vCurPos.ToString()+"--角色2位置:"+curPeer.m_vCurPos.ToString()+ "--:当前角色之间的距离："+dis);
                            //Helper.Log(curPeer.m_curRole.Name + "--:同步:" + peer.m_curRole.Name + "-移动位置信息：" + strRes);
                        }
                    }
                }
            }       
        }

        /// <summary>
        /// 同步角色移动方向
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="peer"></param>
        /// <param name="sendParameters"></param>
        void SyncRoleMoveDir(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Dictionary<byte, object> m_dic = request.Parameters;
            object value;

            // 客户端传递过来的参数信息：角色ID，坐标：X,Y,Z 
            if (m_dic.TryGetValue((byte)ParameterCode.SyncInfo, out value))
            {
                string strValue = value.ToString();
                //Helper.Log("同步角色移动方向:" + strValue);
                string[] pStr1 = strValue.Split(',');
                if (pStr1.Length == 4)
                {
                    if (null == peer.m_myTeam)
                    {
                        return;
                    }

                    List<ClientPeer> pTeam = peer.m_myTeam.m_pTeam;
                    if (pTeam == null)
                    {
                        return;
                    }

                    // 通知队伍中的其他Peer返回队伍角色信息给客户端
                    for (int k = 0; k < pTeam.Count; ++k)
                    {
                        ClientPeer curPeer = pTeam[k];
                        // 九宫格同步
                        bool bSync = Helper.IsSudoKu(peer.m_vCurPos, curPeer.m_vCurPos);

                        if (curPeer != peer && bSync)
                        {
                            EventData eventData = new EventData();
                            eventData.Parameters = m_dic;
                            // 返回操作码
                            eventData.Code = (byte)OperationCode.SyncMoveDir;
                            curPeer.SendEvent(eventData, new SendParameters());
                            //Helper.Log(curPeer.m_curRole.Name + "--:同步:" + peer.m_curRole.Name + "-移动方向：" + strValue);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 同步角色移动动画
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="peer"></param>
        /// <param name="sendParameters"></param>
        void SyncRoleMoveAnim(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Dictionary<byte, object> m_dic = request.Parameters;
            object value;
            // 客户端传递过来的参数信息：角色ID，坐标：X,Y,Z  Y轴旋转:_rY
            if (m_dic.TryGetValue((byte)ParameterCode.SyncInfo, out value))
            {
                string strValue = value.ToString();
                Helper.Log("SyncRoleMoveAnim:" + strValue);
                string[] pStr1 = strValue.Split(',');
                if (pStr1.Length == 2)
                {
                    if (null == peer.m_myTeam)
                    {
                        return;
                    }
                    List<ClientPeer> pTeam = peer.m_myTeam.m_pTeam;
                    if (pTeam == null)
                    {
                        return;
                    }
                    // 通知队伍中的其他Peer返回队伍角色信息给客户端
                    for (int k = 0; k < pTeam.Count; ++k)
                    {
                        ClientPeer curPeer = pTeam[k];
                        if (curPeer != peer)
                        {
                            EventData eventData = new EventData();
                            eventData.Parameters = m_dic;
                            // 返回操作码
                            eventData.Code = (byte)OperationCode.SyncMoveAnim;
                            curPeer.SendEvent(eventData, new SendParameters());
                            //Helper.Log(curPeer.m_curRole.Name + "--:同步:"+peer.m_curRole.Name+"-移动动画信息：" + strValue);
                        }
                    }
                }
            }   
        }

        /// <summary>
        /// 队伍玩家掉线，剔除
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="peer"></param>
        /// <param name="sendParameters"></param>
        public void RemoveTeamRole(ClientPeer peer)
        {
            if (null == peer.m_myTeam)
            {
                return;
            }
            List<ClientPeer> pTeam = peer.m_myTeam.m_pTeam;
            if (null == pTeam)
            {
                return;
            }
            for (int k = 0; k < pTeam.Count; ++k)
            {
                ClientPeer curPeer = pTeam[k];
                if (curPeer != peer)
                {
                    EventData eventData = new EventData();
                    Dictionary<byte, object> dic = new Dictionary<byte, object>();
                    string strID = peer.m_curRole.Id.ToString();
                    dic.Add((byte)ParameterCode.SyncInfo,strID);
                    eventData.Parameters = dic;
                    // 返回操作码
                    eventData.Code = (byte)OperationCode.RemoveTeamRole;
                    curPeer.SendEvent(eventData, new SendParameters());
                    Helper.Log(curPeer.m_curRole.Name + "--移除玩家：" + peer.m_curRole.Name);
                    // 队伍中的其他Peer移除当前peer
                    curPeer.m_myTeam.m_pTeam.Remove(peer);
                }
            }
            peer.m_myTeam.m_pTeam = null;
        }
    }
}
