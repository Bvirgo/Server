using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameServer.DataBase.Model
{
    public class User
    {
        #region MySQL Data
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Password { get; set; }
        #endregion

        public Role role;

        public RoleTempData tempData;

        public ClientPeer Peer;

        public User(ClientPeer _peer)
        {
            Peer = _peer;
        }

        //发送
        public void Send(ProtocolBase proto)
        {
            if (Peer == null)
                return;
            SocketServer.Instance.Send(Peer, proto);
        }

        //踢下线
        public static bool KickOff(int id, ProtocolBase proto)
        {
            ClientPeer[] conns = SocketServer.Instance.conns;
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                    continue;
                if (!conns[i].isUse)
                    continue;
                if (conns[i].m_curUser == null)
                    continue;
                if (conns[i].m_curUser.Id == id)
                {
                    lock (conns[i].m_curUser)
                    {
                        if (proto != null)
                            conns[i].m_curUser.Send(proto);

                        return conns[i].m_curUser.Logout();
                    }
                }
            }
            return true;
        }

        //下线
        public bool Logout()
        {
            //事件处理，稍后实现
            //SocketServer.Instance.handlePlayerEvent.OnLogout(this);
            //保存
            //if (!DataMgr.instance.SavePlayer(this))
            //{
            //    return false;
            //}
            //下线
            Peer.m_curUser = null;
            Peer.Close();
            return true;
        }
    }
}
