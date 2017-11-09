using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGameServer.Handlers;
using MyGameServer.Plus;
using MyGameServer.Tool;

namespace MyGameServer.Plus
{
    // 副本逻辑：可以是一个人副本，也可以是组队副本
    // 组队申请进入副本时候，创建该对象；
    // 创建Monster申请的时候，如果当前peer没有队伍，者单独为该peer创建一个该对象
    public class MyRoomTeam
    {
        #region
        // 队伍客户端
        public List<ClientPeer> m_pPeerList;

        // 是否已经为队伍创建了Monster
        public bool m_bInitMonster;

        // Monster管理器
        private MonsterManager m_monsterMgr;

        #endregion

        public MyRoomTeam(List<ClientPeer> _pPeerList)
        {
            if (_pPeerList == null || _pPeerList.Count < 1)
            {
                Helper.Log("初始化MyRoomTeam失败");
                return;
            }
            m_pPeerList = _pPeerList;
            m_bInitMonster = false;
            m_monsterMgr = new MonsterManager();
            m_monsterMgr.SetTeamPeer(m_pPeerList);
        }

        #region 怪物管理器
        /// <summary>
        /// 添加怪物
        /// </summary>
        /// <param name="curMonster"></param>
        public void AddMonster(Monster curMonster)
        {
            // 保存Monster
            m_monsterMgr.SetMonsterList(curMonster);
        }

        /// <summary>
        /// 初始化怪物出生位置
        /// </summary>
        /// <param name="_strModelID"></param>
        /// <param name="_vPos"></param>
        public void InitMonsterPos(string _strGUID, Vector3 _vPos)
        {
            // 保存Monster初始位置
            m_monsterMgr.SetMonsterPos(_strGUID, _vPos);
        }

        /// <summary>
        /// 设置怪物巡逻点列表
        /// </summary>
        /// <param name="_strModelID"></param>
        /// <param name="_pTriggerPos"></param>
        public void SetMonsterCruise(string _strModelID, List<Vector3> _pTriggerPos)
        {
            // 保存巡航点
            m_monsterMgr.SetCruise(_strModelID, _pTriggerPos);
        }

        /// <summary>
        /// 启动该队伍Monster定时器，处理怪物逻辑
        /// </summary>
        public void StartTimer()
        {
            // 启动定时器
            m_monsterMgr.SetTimer();
        }

        /// <summary>
        /// 通知各个客户端创建怪物
        /// </summary>
        /// <param name="_dic"></param>
        public void SendCreateMonsterEvent(Dictionary<byte, object> _dic)
        {
            m_monsterMgr.SendCreateMonsterEvent(_dic);
        }
        #endregion
       
    }
}
