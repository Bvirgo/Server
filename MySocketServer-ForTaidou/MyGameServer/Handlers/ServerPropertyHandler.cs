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
namespace MyGameServer.Handlers
{
    public class ServerPropertyHandler:HandlerBase
    {
        private static readonly ILogger log = ExitGames.Logging.LogManager.GetCurrentClassLogger();
        private ServerPropertyMgr m_curMgr;
        public ServerPropertyHandler()
        {
            m_curMgr = new ServerPropertyMgr();
        }

        public override void OnHandlerMessage(OperationRequest request,
            OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            // 同步当前选择的服务器
            Dictionary<byte, object> m_dic = request.Parameters;
            object value;
            if (m_dic.TryGetValue((byte)ParameterCode.CurServerID,out value))
            {
                peer.m_nCurServer = Helper.IntParse(value.ToString());
                log.Debug("同步当前选择的服务器ID：" + value);
                return;
            }

            // 获取服务器列表
            List<ServerProperty> serverList = m_curMgr.GetAllServer();
            // list转为Json传送给客户端
            string strJson = JsonMapper.ToJson(serverList);
            log.Debug("ServerPropertyHandle Get ServerList!!!!");
            log.Debug("操作代码：" + request.OperationCode + "--定义代码：" + OperationCode.GetServer);
            log.Debug("服务器查询出来的数据：" + strJson);

            string strResult = "";
            for (int i = 0; i < serverList.Count;++i )
            {
                ServerProperty curServer = serverList[i];
                strResult += "|" + curServer.ID+","+curServer.Name+","+curServer.IP+","+curServer.Count;
            }


            Dictionary<byte, object> parameters = response.Parameters;
            //parameters.Add((byte)ParameterCode.ServerList, strJson);
            parameters.Add((byte)ParameterCode.ServerList, strResult);

            response.OperationCode = request.OperationCode;
        }
    }
}
