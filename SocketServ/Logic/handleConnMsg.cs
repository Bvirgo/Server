using LitJson;
using System;
using System.Collections.Generic;

public partial class HandleConnMsg
{
	//心跳
	//协议参数：无
	public void MsgHeatBeat(Conn conn, ProtocolBase protoBase)
	{
		conn.lastTickTime = Sys.GetTimeStamp();
		Console.WriteLine("[更新心跳时间]" + conn.GetAdress());
	}

	//注册
	//协议参数：str用户名,str密码
	//返回协议：-1表示失败 0表示成功
	public void MsgRegister(Conn conn, ProtocolBase protoBase)
	{
		//获取数值
		int start = 0;
		ProtocolBytes protocol = (ProtocolBytes)protoBase;
		string protoName = protocol.GetString (start, ref start);
		string id = protocol.GetString (start, ref start);
		string pw = protocol.GetString (start, ref start);
		string strFormat = "[收到注册协议]" + conn.GetAdress();
		Console.WriteLine ( strFormat  + " 用户名：" + id + " 密码：" + pw);
		//构建返回协议
		protocol = new ProtocolBytes ();
		protocol.AddString ("Register");
		//注册
		if(DataMgr.instance.Register (id, pw))
		{
			protocol.AddInt(0);
		}
		else
		{
			protocol.AddInt(-1);
		}
		//创建角色
		DataMgr.instance.CreatePlayer (id);
		//返回协议给客户端
		conn.Send (protocol);
	}

	//登录
	//协议参数：str用户名,str密码
	//返回协议：-1表示失败 0表示成功
	public void MsgLogin(Conn conn, ProtocolBase protoBase)
	{
		//获取数值
		int start = 0;
		ProtocolBytes protocol = (ProtocolBytes)protoBase;
		string protoName = protocol.GetString (start, ref start);
		string id = protocol.GetString (start, ref start);
		string pw = protocol.GetString (start, ref start);
		string strFormat = "[收到登录协议]" + conn.GetAdress();
		Console.WriteLine (strFormat  + " 用户名：" + id + " 密码：" + pw);
		//构建返回协议
		ProtocolBytes protocolRet = new ProtocolBytes ();
		protocolRet.AddString ("Login");
		//验证
		if (!DataMgr.instance.CheckPassWord (id, pw)) 
		{
			protocolRet.AddInt(-1);
			conn.Send (protocolRet);
			return;
		}
		//是否已经登录
		ProtocolBytes protocolLogout = new ProtocolBytes ();
		protocolLogout.AddString ("Logout");
		if (!Player.KickOff (id, protocolLogout)) 
		{
			protocolRet.AddInt(-1);
			conn.Send (protocolRet);
			return;
		}

		//获取玩家数据
		PlayerData playerData = DataMgr.instance.GetPlayerData (id);
		if (playerData == null)
		{
			protocolRet.AddInt(-1);
			conn.Send (protocolRet);
			return;
		}
		conn.player = new Player (id, conn);
		conn.player.data = playerData;
		//事件触发
		ServNet.instance.handlePlayerEvent.OnLogin(conn.player);

        // 正常返回
        //protocolRet.AddInt(0);
        //protocolRet.AddString(id);
        //conn.Send(protocolRet);

        // 测试 Json
        RspMsg msg = new RspMsg();
        msg.rspType = 0;
        msg.strTips = "登录成功！";
        TMsg tg = new TMsg();
        msg.strJsData = JsonMapper.ToJson(tg);

        ProtocolJson ptemp = new ProtocolJson();
        ptemp.SetKeyCode("Login");
        ptemp.PushContent<RspMsg>(msg);
        conn.Send(ptemp);

        //// 测试probuffer
        //ProtocolPB pb = new ProtocolPB();
        //pb.SetKeyCode("Login");
        //ChatMsg cm = new ChatMsg();
        //cm.sender = "习近平:";
        //cm.msg = "特朗普今日访华,拉出去宰了！！";
        //List<string> pData = new List<string>();
        //for (int i = 0; i < 10; i++)
        //{
        //    pData.Add(i.ToString());
        //}
        //cm.data = pData;
        //TMsg tm = new TMsg();
        //cm.content = tm;
        //pb.Serialize<ChatMsg>(cm);
        //conn.Send(pb);
        return;
	}

	//下线
	//协议参数：
	//返回协议：0-正常下线
	public void MsgLogout(Conn conn, ProtocolBase protoBase)
	{
		ProtocolBytes protocol = new ProtocolBytes ();
		protocol.AddString ("Logout");
		protocol.AddInt (0);
		if (conn.player == null) 
		{
			conn.Send (protocol);
			conn.Close ();
		}
		else 
		{
			conn.Send (protocol);
			conn.player.Logout();
		}
	}
}