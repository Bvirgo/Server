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
using System.Net.Sockets;
using System.Net;

namespace MyGameServer
{
    public class SocketServer
    {
        #region Logic 
        // handlers管理:每一个OperationCode对应一个hander
        public Dictionary<byte, HandlerBase> m_DicHandler = new Dictionary<byte, HandlerBase>();

        // 申请组队的客户端列表:跨服的，没有区分服务器差异
        private List<ClientPeer> m_pPeerForTeam = new List<ClientPeer>();

        // 副本队伍申请字典
        private Dictionary<string, List<ClientPeer>> m_dicForTeam = new Dictionary<string, List<ClientPeer>>();

        // 已经创建过的ClientPeer列表
        public List<ClientPeer> m_pInitMonsterPeer = new List<ClientPeer>();

        public string m_strBinaryPath;

        protected ClientPeer CreatePeer()
        {
            ClientPeer curPeer = new ClientPeer();
            // 注册Handler
            RegisteHandler((byte)OperationCode.GetServer, new ServerPropertyHandler());
            RegisteHandler((byte)OperationCode.Login, new LoginHandler());
            RegisteHandler((byte)OperationCode.UserRegiste, new UserRegisteHandler());
            RoleHandler roleHander = new RoleHandler();
            RegisteHandler((byte)OperationCode.RoleHandler, roleHander);
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
            RegisteHandler((byte)OperationCode.GetSkill, skillhandler);
            RegisteHandler((byte)OperationCode.AddSkill, skillhandler);
            RegisteHandler((byte)OperationCode.UpdateSkill, skillhandler);

            BattleHandler battleHandler = new BattleHandler();
            RegisteHandler((byte)OperationCode.ForTeam, battleHandler);
            RegisteHandler((byte)OperationCode.CancelTeam, battleHandler);
            RegisteHandler((byte)OperationCode.SyncMove, battleHandler);
            RegisteHandler((byte)OperationCode.SyncMoveAnim, battleHandler);
            RegisteHandler((byte)OperationCode.RemoveTeamRole, battleHandler);
            RegisteHandler((byte)OperationCode.SyncMoveDir, battleHandler);

            MonsterHandler monsterHandler = new MonsterHandler();
            RegisteHandler((byte)OperationCode.CreateMonster, monsterHandler);
            RegisteHandler((byte)OperationCode.SyncMonsterMove, monsterHandler);
            return curPeer;
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
            m_DicHandler.Add(_code, _handler);
        }
        #endregion

        #region Socket

        //监听嵌套字
        public Socket listenfd;
        //客户端链接
        public ClientPeer[] conns;
        //最大链接数
        public int maxConn = 50;
        //单例
        public static SocketServer Instance;
        //主定时器
        System.Timers.Timer timer = new System.Timers.Timer(1000);
        //心跳时间
        public long heartBeatTime = 180;
        //协议
        public ProtocolBase proto;
        //消息分发
        //public HandleConnMsg handleConnMsg = new HandleConnMsg();
        //public HandlePlayerMsg handlePlayerMsg = new HandlePlayerMsg();
        //public HandlePlayerEvent handlePlayerEvent = new HandlePlayerEvent();
        
        public SocketServer()
        {
            Console.WriteLine("Server Manager Init");
            Instance = this;
        }

        //获取链接池索引，返回负数表示获取失败
        public int NewIndex()
        {
            if (conns == null)
                return -1;
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                {
                    conns[i] = new ClientPeer();
                    return i;
                }
                else if (conns[i].isUse == false)
                {
                    return i;
                }
            }
            return -1;
        }

        //开启服务器
        public void Start(string host, int port)
        {
            //定时器
            timer.Elapsed += new System.Timers.ElapsedEventHandler(HandleMainTimer);
            timer.AutoReset = false;
            timer.Enabled = true;
            //链接池
            conns = new ClientPeer[maxConn];
            for (int i = 0; i < maxConn; i++)
            {
                conns[i] = new ClientPeer();
            }
            //Socket
            listenfd = new Socket(AddressFamily.InterNetwork,
                                  SocketType.Stream, ProtocolType.Tcp);
            //Bind
            IPAddress ipAdr = IPAddress.Parse(host);
            IPEndPoint ipEp = new IPEndPoint(ipAdr, port);
            listenfd.Bind(ipEp);
            //Listen
            listenfd.Listen(maxConn);
            //Accept
            listenfd.BeginAccept(AcceptCb, null);
            Console.WriteLine("[服务器]启动成功");
        }


        //Accept回调
        private void AcceptCb(IAsyncResult ar)
        {
            try
            {
                Socket socket = listenfd.EndAccept(ar);
                int index = NewIndex();

                if (index < 0)
                {
                    socket.Close();
                    Console.Write("[警告]链接已满");
                }
                else
                {
                    ClientPeer conn = conns[index];
                    conn.Init(socket);
                    string adr = conn.GetAdress();
                    Console.WriteLine("客户端连接 [" + adr + "] conn池ID：" + index);
                    conn.socket.BeginReceive(conn.readBuff,
                                             conn.buffCount, conn.BuffRemain(),
                                             SocketFlags.None, ReceiveCb, conn);
                }
                listenfd.BeginAccept(AcceptCb, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("AcceptCb失败:" + e.Message);
            }
        }

        //关闭
        public void Close()
        {
            for (int i = 0; i < conns.Length; i++)
            {
                ClientPeer conn = conns[i];
                if (conn == null) continue;
                if (!conn.isUse) continue;
                lock (conn)
                {
                    conn.Close();
                }
            }
        }

        private void ReceiveCb(IAsyncResult ar)
        {
            ClientPeer conn = (ClientPeer)ar.AsyncState;
            lock (conn)
            {
                try
                {
                    int count = conn.socket.EndReceive(ar);
                    //关闭信号
                    if (count <= 0)
                    {
                        Console.WriteLine("收到 [" + conn.GetAdress() + "] 断开链接");
                        conn.Close();
                        return;
                    }
                    conn.buffCount += count;
                    ProcessData(conn);
                    //继续接收	
                    conn.socket.BeginReceive(conn.readBuff,
                                             conn.buffCount, conn.BuffRemain(),
                                             SocketFlags.None, ReceiveCb, conn);
                }
                catch (Exception e)
                {
                    Console.WriteLine("收到 [" + conn.GetAdress() + "] 断开链接 " + e.Message);
                    conn.Close();
                }
            }
        }

        private void ProcessData(ClientPeer conn)
        {
            //小于长度字节
            if (conn.buffCount < sizeof(Int32))
            {
                return;
            }
            //消息长度
            Array.Copy(conn.readBuff, conn.lenBytes, sizeof(Int32));
            conn.msgLength = BitConverter.ToInt32(conn.lenBytes, 0);
            if (conn.buffCount < conn.msgLength + sizeof(Int32))
            {
                return;
            }
            //处理消息
            ProtocolBase protocol = proto.Decode(conn.readBuff, sizeof(Int32), conn.msgLength);
            HandleMsg(conn, protocol);
            //清除已处理的消息
            int count = conn.buffCount - conn.msgLength - sizeof(Int32);
            Array.Copy(conn.readBuff, sizeof(Int32) + conn.msgLength, conn.readBuff, 0, count);
            conn.buffCount = count;
            if (conn.buffCount > 0)
            {
                ProcessData(conn);
            }
        }

        private void HandleMsg(ClientPeer conn, ProtocolBase protoBase)
        {
            string name = protoBase.GetName();
            string methodName = "Msg" + name;
            //连接协议分发
            //if (conn.player == null || name == "HeatBeat" || name == "Logout")
            //{
            //    MethodInfo mm = handleConnMsg.GetType().GetMethod(methodName);
            //    if (mm == null)
            //    {
            //        string str = "[警告]HandleMsg没有处理连接方法 ";
            //        Console.WriteLine(str + methodName);
            //        return;
            //    }
            //    Object[] obj = new object[] { conn, protoBase };
            //    Console.WriteLine("[处理链接消息]" + conn.GetAdress() + " :" + name);
            //    mm.Invoke(handleConnMsg, obj);
            //}
            ////角色协议分发
            //else
            //{
            //    MethodInfo mm = handlePlayerMsg.GetType().GetMethod(methodName);
            //    if (mm == null)
            //    {
            //        string str = "[警告]HandleMsg没有处理玩家方法 ";
            //        Console.WriteLine(str + methodName);
            //        return;
            //    }
            //    Object[] obj = new object[] { conn.player, protoBase };
            //    Console.WriteLine("[处理玩家消息]" + conn.player.id + " :" + name);
            //    mm.Invoke(handlePlayerMsg, obj);
            //}
        }

        //发送
        public void Send(ClientPeer conn, ProtocolBase protocol)
        {
            byte[] bytes = protocol.Encode();
            byte[] length = BitConverter.GetBytes(bytes.Length);
            byte[] sendbuff = length.Concat(bytes).ToArray();
            try
            {
                conn.socket.BeginSend(sendbuff, 0, sendbuff.Length, SocketFlags.None, null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("[发送消息]" + conn.GetAdress() + " : " + e.Message);
            }
        }

        //广播
        public void Broadcast(ProtocolBase protocol)
        {
            for (int i = 0; i < conns.Length; i++)
            {
                if (!conns[i].isUse)
                    continue;
                if (conns[i].m_curUser == null)
                    continue;
                Send(conns[i], protocol);
            }
        }

        //主定时器
        public void HandleMainTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            //处理心跳
            HeartBeat();
            timer.Start();
        }

        //心跳
        public void HeartBeat()
        {
            //Console.WriteLine ("[主定时器执行]");
            long timeNow = Sys.GetTimeStamp();

            for (int i = 0; i < conns.Length; i++)
            {
                ClientPeer conn = conns[i];
                if (conn == null) continue;
                if (!conn.isUse) continue;

                if (conn.lastTickTime < timeNow - heartBeatTime)
                {
                    Console.WriteLine("[心跳引起断开连接]" + conn.GetAdress());
                    lock (conn)
                        conn.Close();
                }
            }
        }

        //打印信息
        public void Print()
        {
            Console.WriteLine("===服务器登录信息===");
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                    continue;
                if (!conns[i].isUse)
                    continue;

                string str = "连接[" + conns[i].GetAdress() + "] ";
                if (conns[i].m_curUser != null)
                    str += "玩家id " + conns[i].m_curUser.Id;

                Console.WriteLine(str);
            }
        }
        #endregion
    }
}
