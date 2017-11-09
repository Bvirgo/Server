using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameServer.DataBase.Model
{
    /// <summary>
    /// Role Template Data Dont Save
    /// </summary>
    public class RoleTempData
    {
        //  状态
        public enum Status
        {
            None,
            Room,
            Fight,
        }
        public Status status;

        public RoleTempData()
        {
            status = Status.None;
        }

        //room状态
        public Room room;
        public int team = 1;
        public bool isOwner = false;

        //战场相关
        public long lastUpdateTime;
        public float posX;
        public float posY;
        public float posZ;
        public long lastShootTime;
        public float hp = 100;
    }
}
