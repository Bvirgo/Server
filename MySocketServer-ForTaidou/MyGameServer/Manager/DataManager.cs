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
    public class DataManager
    {
        // 单例
        public static DataManager Instance = null;
        // 副本配置信息
        public RoomConfigData m_roomInfo = new RoomConfigData();
        // Monster配置
        public MonsterConfigData m_monsterData = new MonsterConfigData();

        public DataManager()
        {
            // 副本配置表
            string strBinaryPath = MyGameApplication.MyInstance.m_strBinaryPath;
            string strPath = (Path.Combine(strBinaryPath, "RoomConfig.xml"));
            m_roomInfo = Helper.LoadXML<RoomConfigData>(strPath);
            // Monster配置表
            strPath = (Path.Combine(strBinaryPath, "MonsterConfig.xml"));
            m_monsterData = Helper.LoadXML<MonsterConfigData>(strPath);

            Instance = this;
        }

        /// <summary>
        /// 根据副本ID，获取角色生成位置
        /// </summary>
        /// <param name="_strRoomID"></param>
        /// <returns></returns>
        public List<RoleSpawn> GetRoomSpawn(string _strRoomID)
        {
            RoomInfo targetRoom = null;
            for (int i = 0; i < m_roomInfo.m_pRoom.Count; ++i)
            {
                RoomInfo curRoom = m_roomInfo.m_pRoom[i];
                if (curRoom.m_nID.ToString().Equals(_strRoomID))
                {
                    targetRoom = curRoom;
                    break;
                }
            }
            if (targetRoom != null)
            {
                return targetRoom.m_pSpawn;
            }
            else
            {
                return null;
            }
        }
    }
}
