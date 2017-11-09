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
    // 处理登录请求的Handler
    public class LoginHandler : HandlerBase
    {
        private UserLoginDataMgr m_mgr;
        private static readonly ILogger log = ExitGames.Logging.LogManager.GetCurrentClassLogger();
        public LoginHandler()
        {
            m_mgr = new UserLoginDataMgr();
        }

        public override void OnHandlerMessage(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Dictionary<byte, object> m_dic = request.Parameters;
            object value;
            string strUserName;
            string strUserPassword;
            if (m_dic.TryGetValue((byte)ParameterCode.UserLogin, out value))
            {
                
                string strValue = value.ToString();
                strValue.Trim();
                log.Debug("LoginHandle Get Data：" + strValue);
                string[] pStr = strValue.Split(',');
                if (pStr.Length == 2)
                {
                    strUserName = pStr[0];
                    strUserPassword = pStr[1];
                    log.Debug("UserName：" + strUserName + "--UserPassword:" + strUserPassword);
                    List<User> puser = m_mgr.GetUserByName(strUserName);
                    if (puser != null && puser.Count > 0)
                    {
                        // 保存在数据中的密码是MD5加密过的
                        if (puser[0].Password.Equals(MD5Tool.StringToMD5(strUserPassword)))
                        //if (puser[0].Password.Equals(strUserPassword))
                        {
                            response.ReturnCode = (short)ReturnCode.Success;
                            peer.m_curUser = puser[0];
                            Dictionary<byte, object> parameters = response.Parameters;
                            //parameters.Add((byte)ParameterCode.ServerList, strJson);
                            parameters.Add((byte)ParameterCode.UserLogin, puser[0].Id.ToString());
                            log.Debug("Login Success");
                            return;
                        }
                    }
                }
            }

            response.ReturnCode = (short)ReturnCode.Error;
            log.Debug("Login Error");
        }
    }
}
