using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using MyGameServer.DataBase.Manager;
using MyGameCommon;
using ExitGames.Logging;
using MyGameServer.DataBase.Model;
using MyGameServer.Tool;

namespace MyGameServer.Handlers
{
    public class UserRegisteHandler:HandlerBase
    {
        //用户信息表manager
        private UserLoginDataMgr m_mgr;
        //日志
        private static readonly ILogger log = ExitGames.Logging.LogManager.GetCurrentClassLogger();

        public UserRegisteHandler()
        {
            m_mgr = new UserLoginDataMgr();
        }
        public override void OnHandlerMessage(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Dictionary<byte, object> m_dic = request.Parameters;
            object value;
            string strUserName;
            string strUserPassword;
            if (m_dic.TryGetValue((byte)ParameterCode.UserRegiste, out value))
            {

                string strValue = value.ToString();
                strValue.Trim();
                log.Debug("UserRegisteHandler Get Data：" + strValue);
                string[] pStr = strValue.Split(',');
                if (pStr.Length == 2)
                {
                    strUserName = pStr[0];
                    strUserPassword = pStr[1];
                    log.Debug("RegisterName：" + strUserName + "--RegisterPassword:" + strUserPassword);
                    List<User> puser = m_mgr.GetUserByName(strUserName);
                    // 注册失败:用户名重复
                    if (puser != null && puser.Count > 0)
                    {
                        response.ReturnCode = (short)ReturnCode.Error;
                        response.DebugMessage = "用户名重复!";
                        return;
                    }
                    else
                    {
                        User newUser = new User();
                        newUser.Name = strUserName;
                        string strMD5 = MD5Tool.StringToMD5(strUserPassword);
                        newUser.Password = strMD5;
                        int nID = m_mgr.AddUser(newUser);
                        peer.m_curUser = newUser;
                        Dictionary<byte, object> dic = new Dictionary<byte, object>();
                        dic.Add((byte)ParameterCode.UserRegiste,nID.ToString());
                        Helper.Log("注册新用户："+nID.ToString());
                        response.Parameters = dic;
                        response.ReturnCode = (short)ReturnCode.Success;
                        log.Debug("UserRegiste Success: Add New User:"+ strUserName+"--P:"+strUserPassword+"--MD5:"+strMD5);
                        return;
                    }
                }
            }

            response.ReturnCode = (short)ReturnCode.Error;
            log.Debug("Login Error");
        }
    }
}
