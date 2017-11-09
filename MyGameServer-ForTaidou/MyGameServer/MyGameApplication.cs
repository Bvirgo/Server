using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Photon.SocketServer;
using ExitGames.Logging;
using log4net;
using ExitGames.Logging.Log4Net;
using log4net.Config;
using MyGameCommon;
using MyGameServer.Handlers;
using MyGameCommon.Model;
using MyGameServer.DataBase.Model;
using MyGameServer.Plus;
using MyGameServer.Tool;
namespace MyGameServer
{
    public class MyGameApplication:ApplicationBase
    {
        // 单例模式
        private static MyGameApplication m_instance;
        // 2、获取配置好的日志对象
        private static readonly ILogger log = ExitGames.Logging.LogManager.GetCurrentClassLogger();
        // handlers管理:每一个OperationCode对应一个hander
        public Dictionary<byte, HandlerBase> m_DicHandler = new Dictionary<byte, HandlerBase>();
        // 申请组队的客户端列表:跨服的，没有区分服务器差异
        private List<ClientPeer> m_pPeerForTeam = new List<ClientPeer>();

        // 副本队伍申请字典
        private Dictionary<string, List<ClientPeer>> m_dicForTeam = new Dictionary<string, List<ClientPeer>>();


        // 已经创建过的ClientPeer列表
        public List<ClientPeer> m_pInitMonsterPeer = new List<ClientPeer>();

        public string m_strBinaryPath;

        // 加载数据管理
        private DataManager m_dataMgr;
        // 单例访问控制
        public static MyGameApplication MyInstance
        {
            get { return m_instance; }
        }

        public MyGameApplication()
        {
            m_instance = this;
            m_strBinaryPath = this.BinaryPath;
            m_dataMgr = new DataManager();
        }

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            ClientPeer curPeer = new ClientPeer(initRequest.Protocol, initRequest.PhotonPeer);
            log.Debug("One Peer is Created!");
            // 注册Handler
            RegisteHandler((byte)OperationCode.GetServer, new ServerPropertyHandler());
            RegisteHandler((byte)OperationCode.Login, new LoginHandler());
            RegisteHandler((byte)OperationCode.UserRegiste, new UserRegisteHandler());
            RoleHandler roleHander = new RoleHandler();
            RegisteHandler((byte)OperationCode.RoleHandler,roleHander );
            RegisteHandler((byte)OperationCode.CreateRole, roleHander);
            RegisteHandler((byte)OperationCode.ChooseRole, roleHander);

            TaskHandler taskHandler = new TaskHandler();
            RegisteHandler((byte)OperationCode.AddTask, taskHandler);
            RegisteHandler((byte)OperationCode.GetTask, taskHandler);
            RegisteHandler((byte)OperationCode.UpdateTask, taskHandler);

            RoleInfoHandler roleinfoHandler = new RoleInfoHandler();
            RegisteHandler((byte)OperationCode.AddRoleInfo, roleinfoHandler);
            RegisteHandler((byte)OperationCode.UpdateRoleInfo, roleinfoHandler);
            RegisteHandler((byte)OperationCode.GetRoleInfo, roleinfoHandler);

            RegisteHandler((byte)OperationCode.AddGoodsList, roleinfoHandler);
            RegisteHandler((byte)OperationCode.UpdateGoodsList, roleinfoHandler);
            RegisteHandler((byte)OperationCode.GetGoodsList, roleinfoHandler);

            RegisteHandler((byte)OperationCode.AddEquipList, roleinfoHandler);
            RegisteHandler((byte)OperationCode.UpdateEquipList, roleinfoHandler);
            RegisteHandler((byte)OperationCode.GetEquipList, roleinfoHandler);

            SkillHandler skillhandler = new SkillHandler();
            RegisteHandler((byte)OperationCode.GetSkill,skillhandler);
            RegisteHandler((byte)OperationCode.AddSkill,skillhandler);
            RegisteHandler((byte)OperationCode.UpdateSkill,skillhandler);

            BattleHandler battleHandler = new BattleHandler();
            RegisteHandler((byte)OperationCode.ForTeam, battleHandler);
            RegisteHandler((byte)OperationCode.CancelTeam, battleHandler);
            RegisteHandler((byte)OperationCode.SyncMove, battleHandler);
            RegisteHandler((byte)OperationCode.SyncMoveAnim, battleHandler);
            RegisteHandler((byte)OperationCode.RemoveTeamRole, battleHandler);
            RegisteHandler((byte)OperationCode.SyncMoveDir, battleHandler);

            MonsterHandler monsterHandler = new MonsterHandler();
            RegisteHandler((byte)OperationCode.CreateMonster,monsterHandler);
            RegisteHandler((byte)OperationCode.SyncMonsterMove,monsterHandler);
            return curPeer;
        }

        protected override void Setup()
        {
            // 1、服务器日志配置
            ExitGames.Logging.LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
            GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(this.ApplicationRootPath, "log");
            GlobalContext.Properties["LogFileName"] = "MG" + this.ApplicationName;
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(this.BinaryPath, "log4net.config")));

            // 日志输出
            log.Debug("Application SetUp Finished!");
        }

        protected override void TearDown()
        {
            log.Debug("Application TearDown!");
        }

        // 注册handler：OperationCode和它对应的Handler对应起来
        public void RegisteHandler(byte _code, HandlerBase _handler)
        {
            HandlerBase curHandler;
            if (m_DicHandler.TryGetValue(_code, out curHandler))
            {
                m_DicHandler.Remove(_code);
            }
            // 注册登录Handler
            m_DicHandler.Add(_code,_handler);
        }

        /// <summary>
        /// 客户端申请进入副本
        /// </summary>
        /// <param name="_strRoomid"></param>
        /// <param name="_peer"></param>
        public void AddTeam(string _strRoomid, ClientPeer _peer)
        {
            List<ClientPeer> pList;
            if (_strRoomid.Equals(""))
            {
                Helper.Log("副本ID为空！");
                return ;
            }
            // 已有副本队伍
            if (m_dicForTeam.TryGetValue(_strRoomid, out pList))
            {
                if (!pList.Contains(_peer))
                {
                    pList.Add(_peer);
                }
            }
            else // 新副本队伍
            {
                pList = new List<ClientPeer>();
                pList.Add(_peer);
                m_dicForTeam.Add(_strRoomid, pList);
            }
        }

        /// <summary>
        /// 获取当前副本组队情况
        /// </summary>
        public List<ClientPeer> GetTeam(string _strRoomid)
        {
            List<ClientPeer> pList;
            if (_strRoomid.Equals(""))
            {
                Helper.Log("副本ID为空！");
                return null;
            }
            if (m_dicForTeam.TryGetValue(_strRoomid, out pList))
            {
                // 申请人数 > 0
                if (pList.Count > 0)
                {
                    return pList;
                }
            }
            return null;
        }

        /// <summary>
        /// 组队成功，移除对应的Peer
        /// </summary>
        /// <param name="_strRoomid"></param>
        /// <param name="_peer"></param>
        public void RemovePeer(string _strRoomid, ClientPeer _peer)
        {
            if (string.IsNullOrEmpty(_strRoomid))
            {
                return;
            }
            List<ClientPeer> pList;
            if (m_dicForTeam.TryGetValue(_strRoomid, out pList))
            {
                if (pList.Contains(_peer))
                {
                    pList.Remove(_peer);
                }
                m_dicForTeam.Remove(_strRoomid);
                m_dicForTeam.Add(_strRoomid, pList);
            }
        }
    }
}
