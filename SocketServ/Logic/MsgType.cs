using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

public class RspMsg
{
    public byte rspType;
    public string strSubKeyCode;
    public string strTips;
    public string strJsData;
}

public enum RspType : byte
{
    None,
    NetError,
    LogicError,
    DataError
}
/// <summary>
/// 消息对象
/// </summary>
[ProtoContract]
public class ChatMsg
{
    [ProtoMember(1)]
    public string sender;//发送者
    [ProtoMember(2)]
    public string msg;//消息
    [ProtoMember(3)]
    public List<string> data
    {
        get;
        set;
    }
    [ProtoMember(4)]
    public object content;
}

public class TMsg
{
    public TMsg()
    {
        name = "Sub TMSG";
        nID = 10;
    }
    public string name;//发送者
    public int nID;//消息
}
