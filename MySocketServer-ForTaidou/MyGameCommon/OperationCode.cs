using System;
using System.Collections.Generic;
using System.Text;

// 定义一些服务器和客户端共用的结构
namespace MyGameCommon
{
    // 操作请求编码
    public enum OperationCode:byte
    {
        Login=0,
        GetServer=1,
        UserRegiste=2,
        RoleHandler=3,
        CreateRole = 4,
        ChooseRole = 5,
        AddTask = 6,
        UpdateTask=7,
        GetTask=8,
        GetRoleInfo =9,
        UpdateRoleInfo=10,
        AddRoleInfo=11,
        GetGoodsList=12,
        UpdateGoodsList = 13,
        AddGoodsList=14,
        GetEquipList=15,
        UpdateEquipList=16,
        AddEquipList=17,
        GetSkill = 18,
        UpdateSkill = 19,
        AddSkill = 20,
        ForTeam = 21, // 组队申请
        CancelTeam = 22, //取消组队
        SyncMove = 23,// 同步玩家运动位置
        SyncMoveAnim=24,// 同步玩家运动动画
        SyncSkillAnim=25, // 同步玩家技能动画
        RemoveTeamRole = 26,// 队伍玩家掉线，剔除 
        CreateMonster = 27,// 创建怪物
        MonsterCruise = 28, // 怪物巡逻
        SyncMonsterMove = 29,  // 怪物位置同步
        MonsterAttack = 30,// 怪物攻击
        MonsterDie = 31,// 怪物死亡
        MonsterDamage = 32,//怪物受伤
        RoomFail = 33, // 角色副本通关失败
        RoomSuccess = 34, // 角色副本通关
        SyncMoveDir = 35 // 角色移动方向同步
    }
}
