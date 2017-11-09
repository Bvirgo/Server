using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using MyGameCommon;
using MyGameServer.Handlers;
using ExitGames.Logging;
using MyGameCommon.Model;
using MyGameServer.DataBase.Model;
using MyGameServer.Plus;
using MyGameServer.Tool;

namespace MyGameServer
{
    public class ClientPeer:PeerBase
    {
        // 2、获取配置好的日志对象
        private static readonly ILogger log = ExitGames.Logging.LogManager.GetCurrentClassLogger();

        // 保存当前客户端登录的User
        public UserLoginData m_curUser = null;

        // 保存当前客户端选择的ServerID
        public int m_nCurServer = -1;

        // 当前选择的角色
        public RoleData m_curRole = null;

        // 所属队伍
        public BattleTeam m_myTeam;

        // 申请进入副本ID
        public string m_strRoomID;

        // 当前角色所在位置
        public Vector3 m_vCurPos = new Vector3(0,0,0);

        // 当前peer所属的MonsterMgr

        public MonsterManager m_monsterManager = null;

        // 当前副本出生点列表
        public List<RoleSpawn> m_pRoleSpawn = null;

        public ClientPeer(IRpcProtocol protocol, IPhotonPeer peer)
            : base(protocol, peer)
        {
            
        }
        protected override void OnDisconnect(PhotonHostRuntimeInterfaces.DisconnectReason reasonCode, string reasonDetail)
        {
            // 断开连接时候需要从队伍中剔除该用户
            MyGameApplication.MyInstance.RemovePeer(m_strRoomID, this);

            // 断开连接时候需要从已经创建了怪物列表中剔除该用户
            if (MyGameApplication.MyInstance.m_pInitMonsterPeer.Contains(this))
            {
                MyGameApplication.MyInstance.m_pInitMonsterPeer.Remove(this);
            }

            // 清除MonsterManager
            if (m_monsterManager != null)
            {
                m_monsterManager.Clear();
            }

            // 玩家掉线，通知队伍玩家删除自己
            HandlerBase curHandler = null;
            // 获取对应的Handler
            MyGameApplication.MyInstance.m_DicHandler.TryGetValue((byte)OperationCode.RemoveTeamRole, out curHandler);
            if (curHandler != null)
            {
                BattleHandler h = (BattleHandler)curHandler;
                h.RemoveTeamRole(this);
            }
            m_curRole = null;
            m_curUser = null;
            log.Debug("A Peer Is Disconnect");
        }

        // 响应客户端请求
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            HandlerBase curHandler = null;
            // 获取对应的Handler
            MyGameApplication.MyInstance.m_DicHandler.TryGetValue(operationRequest.OperationCode,out curHandler);

            OperationResponse response = new OperationResponse();
            response.OperationCode = operationRequest.OperationCode;
            response.Parameters = new Dictionary<byte, object>();

            if (null == curHandler)
            {
                log.Debug("Can't Find OperationCode:" + operationRequest.OperationCode);
                return;
            }
            // 调用Handler处理对应逻辑
            curHandler.OnHandlerMessage(operationRequest,response,this,sendParameters);
            if (response != null) 
            {
                // log.Debug("ClientPeer Deal Success!");
                // 返回给客户端处理结果
                SendOperationResponse(response, sendParameters);
            }

        }
    }
}
