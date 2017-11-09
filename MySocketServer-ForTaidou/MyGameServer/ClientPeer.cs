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
using System.Net.Sockets;

namespace MyGameServer
{
    public class ClientPeer
    {
        #region Socket Data
        //常量
        public const int BUFFER_SIZE = 1024;
        //Socket
        public Socket socket;
        //是否使用
        public bool isUse = false;
        //Buff
        public byte[] readBuff = new byte[BUFFER_SIZE];
        public int buffCount = 0;
        //沾包分包
        public byte[] lenBytes = new byte[sizeof(UInt32)];
        public Int32 msgLength = 0;
        //心跳时间
        public long lastTickTime = long.MinValue;
        #endregion

        #region Logic Data
        // 保存当前客户端登录的User
        public User m_curUser = null;
        

        // 申请进入副本ID
        public string m_strRoomID;
        
        // 当前副本出生点列表
        public List<RoleSpawn> m_pRoleSpawn = null;

        #endregion

        public ClientPeer()
        {
            readBuff = new byte[BUFFER_SIZE];
        }

        //初始化
        public void Init(Socket socket)
        {
            this.socket = socket;
            isUse = true;
            buffCount = 0;
            //心跳处理，稍后实现GetTimeStamp方法
            lastTickTime = Sys.GetTimeStamp();
        }
        //剩余的Buff
        public int BuffRemain()
        {
            return BUFFER_SIZE - buffCount;
        }
        //获取客户端地址
        public string GetAdress()
        {
            if (!isUse)
                return "无法获取地址";
            return socket.RemoteEndPoint.ToString();
        }
        //关闭
        public void Close()
        {
            if (!isUse)
                return;
            if (m_curUser != null)
            {
                //玩家退出处理，稍后实现
                m_curUser.Logout();
                return;
            }
            Console.WriteLine("[断开链接]" + GetAdress());
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            isUse = false;

        }

        //发送协议，相关内容稍后实现
        public void Send(ProtocolBase protocol)
        {
           SocketServer.Instance.Send(this, protocol);
        }
    }
}
