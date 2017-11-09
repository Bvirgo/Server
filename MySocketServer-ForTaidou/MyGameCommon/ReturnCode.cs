using System;
using System.Collections.Generic;
using System.Text;

namespace MyGameCommon
{
    /// <summary>
    /// 服务器返回给客户端的状态信息
    /// </summary>
    public enum ReturnCode:short
    {
        Success,
        Error,
        Fail,
        Exception,
        Waitting
    }
}
