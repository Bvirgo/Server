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

namespace MyGameServer.Plus
{
    public class MonsterManager
    {
        // 怪物列表信息
        private List<Monster> m_pMonsterInfo = new List<Monster>();

        // 怪物巡航点分组<模型类型，巡逻点列表>
        private Dictionary<string, List<Vector3>> m_dicCruiseGroup = new Dictionary<string, List<Vector3>>();

        // Monster位置信息
        private Dictionary<string, Vector3> m_dicMonsterPos = new Dictionary<string, Vector3>();

        // 队伍玩家列表
        private List<ClientPeer> m_pTeamPeer = new List<ClientPeer>();

        // 玩家周围怪物列表
        private List<Monster> m_pAroundMonster = new List<Monster>();

        // 定时器
        System.Timers.Timer t1;

        // 定时器时间间隔
        float fTimerTime = 20f;

        public void SetMonsterList(Monster _monster)
        {
            if (!m_pMonsterInfo.Contains(_monster) && _monster != null)
            {
                m_pMonsterInfo.Add(_monster);
            }
        }

        public void SetCruise(string _strModelID, List<Vector3> _pList)
        {
            if (!m_dicCruiseGroup.ContainsKey(_strModelID) && _pList != null)
            {
                m_dicCruiseGroup.Add(_strModelID, _pList);
            }
        }

        public void SetMonsterPos(string _strGUID, Vector3 _vPos)
        {
            if (m_dicMonsterPos.ContainsKey(_strGUID))
            {
                m_dicMonsterPos.Remove(_strGUID);
            }
            m_dicMonsterPos.Add(_strGUID, _vPos);
            Helper.Log("怪物：" + _strGUID + "--初始坐标:" + Vector3.GetVector(_vPos));
        }

        /// <summary>
        /// 队伍中的角色
        /// </summary>
        /// <param name="_peer"></param>
        public void SetTeamPeer(List<ClientPeer> _pPeer)
        {
            m_pTeamPeer = _pPeer;
            for (int i = 0; i < _pPeer.Count; ++i)
            {
                _pPeer[i].m_monsterManager = this;
            }
        }

        /// <summary>
        /// 通知队伍中的角色创建怪物
        /// </summary>
        public void SendCreateMonsterEvent(Dictionary<byte, object> _dic)
        {

            List<ClientPeer> pTeam = m_pTeamPeer;

            for (int k = 0; k < pTeam.Count; ++k)
            {
                ClientPeer curPeer = pTeam[k];
                EventData eventData = new EventData();
                eventData.Parameters = _dic;
                // 返回操作码
                eventData.Code = (byte)OperationCode.CreateMonster;
                curPeer.SendEvent(eventData, new SendParameters());
                Helper.Log(curPeer.m_curRole.Name + "--:创建怪物");
            }
        }

        /// <summary>
        /// Monster管理：巡航、追击、攻击
        /// </summary>
        public void SetTimer()
        {
            t1 = new System.Timers.Timer(fTimerTime);//实例化Timer类，设置间隔时间为10000毫秒(10s)；
            //t1.Elapsed += new System.Timers.ElapsedEventHandler(Update);//到达时间的时候执行事件；
            //t1.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            //t1.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
        }

        /// <summary>
        /// 客户端断线，清除定时器等状态
        /// </summary>
        public void Clear()
        {
            t1.AutoReset = false;
            t1.Enabled = false;
        }
        /// <summary>
        /// 玩家周围的Monster才需要展示
        /// </summary>
        void GetAroundMonster()
        {
            m_pAroundMonster.Clear();

            for (int j = 0; j < m_pMonsterInfo.Count; ++j)
            {
                Monster curMonster = m_pMonsterInfo[j];
                // 追击距离是攻击距离的两倍
                float pursuitDis = curMonster.m_fAttackDis * 3;
                Vector3 vRolePos;
                float dis = GetMinDis(curMonster, out vRolePos);

                // Monster动作状态：1：站立 2 ：巡航 3：追击 4：攻击 
                if (dis < 23) // 玩家周围怪物
                {
                    if (!m_pAroundMonster.Contains(curMonster))
                    {
                        m_pAroundMonster.Add(curMonster);
                    }
                }
            }
        }

        /// <summary>
        /// 获取怪物和最近玩家之间的距离
        /// </summary>
        float GetMinDis(Monster monster, out Vector3 _vRole)
        {
            float minDis = int.MaxValue;
            _vRole = monster.m_curPos;
            for (int i = 0; i < m_pTeamPeer.Count; ++i)
            {
                ClientPeer curPeer = m_pTeamPeer[i];
                Vector3 vRolePos = curPeer.m_vCurPos;
                Vector3 vMPos = monster.m_curPos;
                float d = Vector3.Distance(vMPos, vRolePos);
                if (d < minDis)
                {
                    minDis = d;
                    _vRole = vRolePos;
                }
            }
            return minDis;
        }

        /// <summary>
        /// 定时器
        /// </summary>
        void Update(object source, System.Timers.ElapsedEventArgs e)
        {
            // 每个怪物取一个巡航点，通知客户端移动
            GetAroundMonster();

            for (int i = 0; i < m_pAroundMonster.Count; ++i)
            {
                Monster curMonster = m_pAroundMonster[i];
                string strModelID = curMonster.m_strModelId;
                string strGuid = curMonster.m_strGuid;
                // Monster动作状态：1：站立 2 ：巡航 3：追击 4：攻击 
                byte bType = curMonster.m_ActionType;
                switch (bType)
                {
                    // 站立状态
                    case 1:
                        MonsterBeginCruise(curMonster);
                        break;
                    // 巡航状态
                    case 2:
                        OnMonsterCruise(curMonster);
                        break;
                    // 攻击状态
                    case 3:

                        break;
                }

            }
        }

        /// <summary>
        /// Monster巡航
        /// </summary>
        /// <param name="_monster"></param>
        void MonsterBeginCruise(Monster _monster)
        {
            Monster curMonster = _monster;
            string strModelID = curMonster.m_strModelId;
            string strGuid = curMonster.m_strGuid;

            Dictionary<byte, object> dic = new Dictionary<byte, object>();
            List<Vector3> pCruise = new List<Vector3>();

            if (pCruise != null)
            {
                pCruise.Clear();
            }
            dic.Clear();
            if (m_dicCruiseGroup.TryGetValue(strModelID, out pCruise))
            {
                List<ClientPeer> pTeam = m_pTeamPeer;

                string strRes;
                strRes = "";
                int n = Helper.Random(0, pCruise.Count);

                Vector3 vTarget = pCruise[n];
                curMonster.m_targetPos = vTarget;
                curMonster.m_ActionType = 2;

                // 返回信息：GUID,X,Y,Z
                strRes = strGuid + "," + vTarget.x.ToString() + "," + vTarget.y.ToString() + "," + vTarget.z.ToString();
                dic.Add((byte)ParameterCode.monsterInfo, strRes);
                Helper.Log("随机数：" + n + "--巡航点类型:" + strModelID + "--随机坐标:" + Vector3.GetVector(vTarget));
                for (int k = 0; k < pTeam.Count; ++k)
                {
                    ClientPeer curPeer = pTeam[k];
                    EventData eventData = new EventData();
                    eventData.Parameters = dic;
                    // 返回操作码
                    eventData.Code = (byte)OperationCode.MonsterCruise;
                    curPeer.SendEvent(eventData, new SendParameters());
                    //Helper.Log(curPeer.m_curRole.Name + "--:"+m_pMonsterInfo[i].m_strModelId+"--:怪物移动:" + strRes);
                }
            }
        }

        /// <summary>
        /// Monster巡航：实时更新坐标
        /// </summary>
        void OnMonsterCruise(Monster _monster)
        {
            Vector3 dir = _monster.m_targetPos - _monster.m_curPos;
            Vector3 v = dir;
            dir = dir.nomalize;
            dir = dir * _monster.m_nCruiseSpeed * 0.02f;
            _monster.m_curPos = _monster.m_curPos + dir;
            //Helper.Log("移动之后的位置："+_monster.m_curPos.ToString() + "-----目标位置:"+_monster.m_targetPos.ToString());
            float dis = Vector3.Distance(_monster.m_curPos, _monster.m_targetPos);
            Helper.Log("Monster巡航：单位向量：" + v.nomalize.ToString() + "--:偏移" + v.ToString() + "--当前位置:" + _monster.m_curPos.ToString() + "--目标位置：" + _monster.m_targetPos + "--余下距离" + dis);
            // 达到目标点：设置为站立状态
            if (dis < 0.3f)
            {
                _monster.m_ActionType = 1;
                _monster.m_curPos = _monster.m_targetPos;
                // 到达巡航目标点，同步一次Monster位置
                SyncMonsterPos(_monster.m_strGuid, _monster.m_curPos, (byte)OperationCode.SyncMonsterMove);
            }
        }
        /// <summary>
        /// 通知队伍客户端，Monster位置信息
        /// </summary>
        public void SyncMonsterPos(string _strGuid, Vector3 _v, byte _operationCode)
        {
            List<ClientPeer> pTeam = m_pTeamPeer;
            Dictionary<byte, object> dic = new Dictionary<byte, object>();
            string strRes;
            strRes = "";
            // 返回信息：GUID,X,Y,Z
            strRes = _strGuid + "," + _v.x.ToString() + "," + _v.y.ToString() + "," + _v.z.ToString();
            dic.Add((byte)ParameterCode.monsterInfo, strRes);

            for (int k = 0; k < pTeam.Count; ++k)
            {
                ClientPeer curPeer = pTeam[k];
                EventData eventData = new EventData();
                eventData.Parameters = dic;
                // 返回操作码
                eventData.Code = _operationCode;
                curPeer.SendEvent(eventData, new SendParameters());
                Helper.Log("达到巡航目标点，同步坐标位置：" + strRes);
            }
        }
    }
}
