using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGameServer.Handlers;
using MyGameServer.Tool;
using MyGameServer;

namespace MyGameServer.Plus
{
    // 队伍信息
    public class BattleTeam
    {
        // 队伍中的peer列表
        public List<ClientPeer> m_pTeam = new List<ClientPeer>();
        public BattleTeam(List<ClientPeer> _pPeer)
        {
            if (_pPeer != null)
            {
                m_pTeam = _pPeer;
                for (int i = 0; i < _pPeer.Count;++i )
                {
                    _pPeer[i].m_myTeam = this;
                }
            }
        }
    }
}
