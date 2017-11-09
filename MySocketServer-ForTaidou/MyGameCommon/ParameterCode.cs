using System;
using System.Collections.Generic;
using System.Text;

namespace MyGameCommon
{
    /// <summary>
    ///参数类型
    /// </summary>
    public enum ParameterCode:byte
    {
        ServerList=0,
        UserLogin=1,
        UserRegiste=2,
        RoleInfo=3,
        CurServerID= 4,
        TaskInfo = 5,//任务相关数据
        RoleData = 6, //获取角色属性、物品、装备信息
        SkillInfo = 7,
        BattleInfo = 8,// 战斗信息
        SyncInfo = 9,
        monsterInfo = 10
    }
}
