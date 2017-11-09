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
using System.Collections;

namespace MyGameServer.Handlers
{
    public class MonsterHandler:HandlerBase
    {
        // 副本配置信息
        private RoomConfigData m_roomInfo = new RoomConfigData();
        // Monster配置
        private MonsterConfigData m_monsterData = new MonsterConfigData();

        // 巡航点(出生点)
        List<Vector3> m_pTriggerPos = new List<Vector3>();

        // 已经创建过的ClientPeer列表
        List<ClientPeer> m_pInitMonsterPeer = MyGameApplication.MyInstance.m_pInitMonsterPeer;

        public MonsterHandler()
        {
            // 副本配置表
            string strBinaryPath = MyGameApplication.MyInstance.m_strBinaryPath;
            string strPath = (Path.Combine(strBinaryPath, "RoomConfig.xml"));
            m_roomInfo = Helper.LoadXML<RoomConfigData>(strPath);
            // Monster配置表
            strPath = (Path.Combine(strBinaryPath, "MonsterConfig.xml"));
            m_monsterData = Helper.LoadXML<MonsterConfigData>(strPath);

        }
        public override void OnHandlerMessage(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            byte operCode = request.OperationCode;
            switch (operCode)
            {
                case (byte)OperationCode.CreateMonster: // 组队申请:最多3人一个队伍，也可以两人
                    OnCreateMonster(request,response,peer,sendParameters);
                    break;
                case (byte)OperationCode.RoomFail: // 角色副本通关失败
                    OnRoomFail(request, response, peer, sendParameters);
                    break;
                case (byte)OperationCode.RoomSuccess: // 角色副本通关
                    OnRoomSuccess(request,response,peer,sendParameters);
                    break;
                case (byte)OperationCode.SyncMonsterMove: // 同步Monster移动
                    break;
            }
        }

        /// <summary>
        /// 创建副本怪物信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="peer"></param>
        /// <param name="sendParameters"></param>
        void OnCreateMonster(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            if (m_pInitMonsterPeer.Contains(peer))
            {
                Helper.Log("已经为:"+peer.m_curRole.Name +"--创建了怪物列表");
                return;
            }

            Dictionary<byte, object> dic = request.Parameters;
            object value;
            if (dic.TryGetValue((byte)ParameterCode.monsterInfo,out value))
            {
                // 为当前peer创建RoomTeam
                List<ClientPeer> pPeerList = new List<ClientPeer>();
                if (null == peer.m_myTeam || peer.m_myTeam.m_pTeam.Count == 0)
                {
                    pPeerList.Add(peer);
                    m_pInitMonsterPeer.Add(peer);
                }
                else
                {
                    pPeerList = peer.m_myTeam.m_pTeam;
                    m_pInitMonsterPeer.AddRange(pPeerList);
                }
                MyRoomTeam myRoomTeam = new MyRoomTeam(pPeerList);

                string strRoomId = value.ToString();
                Helper.Log("接受到副本创建怪物申请：" + strRoomId);
                RoomInfo targetRoom = null;
                for (int i = 0; i < m_roomInfo.m_pRoom.Count; ++i)
                {
                    RoomInfo curRoom = m_roomInfo.m_pRoom[i];
                    if (curRoom.m_nID.ToString().Equals(strRoomId))
                    {
                        targetRoom = curRoom;
                        break;
                    }
                }
                if (targetRoom != null)
                {
                    // 保存角色出生点位置
                    if (null == peer.m_pRoleSpawn)
                    {
                        peer.m_pRoleSpawn = targetRoom.m_pSpawn;
                    }

                    for (int j = 0; j < targetRoom.m_pTrigger.Count; ++j)
                    {
                        TriggerInfo curTrigger = targetRoom.m_pTrigger[j];
                        if (curTrigger != null)
                        {
                            m_pTriggerPos.Clear();
                            List<Vector3> pCruisePos = new List<Vector3>();

                            // MonsterModel ID
                            string strModelID = curTrigger.m_strModelID;
                            int nNum = curTrigger.m_nNum;
                            if (nNum > curTrigger.m_pPosList.Count)
                            {
                                nNum = curTrigger.m_pPosList.Count;
                            }
                            string strRes;
                            strRes = "";
                            // 怪物数据：GUID,ModelID,x,y,z,cruiseID|
                            for (int k = 0; k < nNum; ++k)
                            {
                                TriggerPosInfo curTiggerPos = curTrigger.m_pPosList[k];

                                string strGUIID = Guid.NewGuid().ToString();
                                strRes += strGUIID + "," + strModelID + "," + curTiggerPos.m_fX.ToString() + "," + curTiggerPos.m_fY.ToString() + "," + curTiggerPos.m_fZ.ToString() + ","+curTrigger.m_nID.ToString()+"|";
                                dic.Clear();
                                dic.Add((byte)ParameterCode.monsterInfo,strRes);

                                Vector3 vPos = new Vector3(curTiggerPos.m_fX,curTiggerPos.m_fY,curTiggerPos.m_fZ);
            
                                m_pTriggerPos.Add(vPos);
                                pCruisePos.Add(vPos);
                                // GetMonsterInfo
                                Monster curMonster = GetMonster(strModelID);
                                if (curMonster != null)
                                {
                                    curMonster.m_strGuid = strGUIID;
                                    curMonster.m_curPos = vPos;
                                    curMonster.m_targetPos = vPos;
                                    curMonster.m_ActionType = 1;
                                }

                                // 保存Monster
                                myRoomTeam.AddMonster(curMonster);

                                // 保存Monster初始位置
                                myRoomTeam.InitMonsterPos(strGUIID,vPos);
                            }

                            // 保存巡航点
                            myRoomTeam.SetMonsterCruise(strModelID,pCruisePos);

                            // 通知队伍中客户端创建该怪物出生点的怪物
                            myRoomTeam.SendCreateMonsterEvent(dic);
                        }
                    }

                    // 开启定时器
                    // myRoomTeam.StartTimer();

                }
            }     
        }

        Monster GetMonster(string _strModelID)
        {
            if (m_monsterData == null)
            {
                Helper.Log("MonsterConfig.xml读取失败！");
                return null;
            }

            for (int i = 0; i < m_monsterData.m_pMonster.Count;++i )
            {
                MonsterInfo curM = m_monsterData.m_pMonster[i];
                if (curM.m_strModelId.Equals(_strModelID))
                {
                    Monster resMonster = new Monster(curM);
                    return resMonster;
                }
            }
            return null;
        }

        /// <summary>
        /// 副本通关失败
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="peer"></param>
        /// <param name="sendParameters"></param>
        void OnRoomFail(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            if (MyGameApplication.MyInstance.m_pInitMonsterPeer.Contains(peer))
            {
                MyGameApplication.MyInstance.m_pInitMonsterPeer.Remove(peer);
            }
        }

        /// <summary>
        /// 副本通关
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="peer"></param>
        /// <param name="sendParameters"></param>
        void OnRoomSuccess(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            if (MyGameApplication.MyInstance.m_pInitMonsterPeer.Contains(peer))
            {
                MyGameApplication.MyInstance.m_pInitMonsterPeer.Remove(peer);
            }
        }

        /// <summary>
        /// 同步Monster移动
        /// </summary>
        void OnSyncMonstertMove(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Dictionary<byte, object> m_dic = request.Parameters;
            object value;
            // 客户端传递过来的参数信息：guid，坐标：X,Y,Z
            if (m_dic.TryGetValue((byte)ParameterCode.monsterInfo, out value))
            {
                string strValue = value.ToString();
                //Helper.Log("SyncRoleMove:" + strValue);
                string[] pStr1 = strValue.Split(',');
                if (pStr1.Length == 4)
                {
                    // 保存角色位置信息
                    float x = float.Parse(pStr1[1]);
                    float y = float.Parse(pStr1[2]);
                    float z = float.Parse(pStr1[3]);
                    Vector3 vPos = new Vector3(x,y,z);
                    string strGuid = pStr1[0];
                }
            }
        }
    }
}
