using System;
using System.Collections;
using System.Linq;
using LitJson;

//字节流协议模型
public class ProtocolJson : ProtocolBase
{
	//传输的字节流
	public byte[] bytes;
	
	//解码器
	public override ProtocolBase Decode(byte[] readbuff, int start, int length)
	{
        ProtocolJson protocol = new ProtocolJson();
		protocol.bytes = new byte[length];
		Array.Copy(readbuff, start, protocol.bytes, 0, length);
		return protocol;
	}
	
	//编码器
	public override byte[] Encode()
	{
		return bytes;
	}
	
	//协议名称
	public override string GetName()
	{
		return GetString(0);
	}
	
	//描述
	public override string GetDesc()
	{
		string str = "";
		if (bytes == null) return str;
		for (int i = 0; i < bytes.Length; i++)
		{
			int b = (int)bytes[i];
			str += b.ToString() + " ";
		}
		return str;
	}

    /// <summary>
    /// 协议Key
    /// </summary>
    /// <param name="_strKey"></param>
    public void SetKeyCode(string _strKey)
    {
        AddString(_strKey);
    }

	//添加字符串
	private void AddString(string str)
	{
		Int32 len = str.Length;
		byte[] lenBytes = BitConverter.GetBytes (len);
		byte[] strBytes = System.Text.Encoding.UTF8.GetBytes (str);
		if(bytes == null)
			bytes = lenBytes.Concat(strBytes).ToArray();
		else
			bytes = bytes.Concat(lenBytes).Concat(strBytes).ToArray();
	}

    /// <summary>
    /// 添加协议内容：Json格式，float请用double代替
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_conten"></param>
    public void PushContent<T>(T _conten)
    {
        string strJsonData;
        try
        {
            strJsonData = JsonMapper.ToJson(_conten);
        }
        catch (Exception e)
        {
            strJsonData = "To Json ERROR:"+e; 
        }
        AddString(strJsonData);
    }

    /// <summary>
    /// 获取Json数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetContent<T>()
    {
        T conten = default(T);
        int nStart = 0;
        GetString(nStart, ref nStart);
        string strJson = GetString(nStart,ref nStart);
        try
        {
            conten = JsonMapper.ToObject<T>(strJson);
        }
        catch (Exception e)
        {
            Console.WriteLine("Json To Object Error:"+e);
        }
        return conten;
    }
	
	//从字节数组的start处开始读取字符串
	private string GetString(int start, ref int end)
	{
		if (bytes == null)
			return "";
		if (bytes.Length < start + sizeof(Int32))
			return "";
		Int32 strLen = BitConverter.ToInt32 (bytes, start);
		if (bytes.Length < start + sizeof(Int32) + strLen)
			return "";
		string str = System.Text.Encoding.UTF8.GetString(bytes,start + sizeof(Int32),strLen);
		end = start + sizeof(Int32) + strLen;
		return str;
	}
	
	private string GetString(int start)
	{
		int end = 0;
		return GetString (start, ref end);
	}
}