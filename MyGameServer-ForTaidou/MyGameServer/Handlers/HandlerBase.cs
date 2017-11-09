using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGameServer;

namespace MyGameServer.Handlers
{
    // 每一个OperationCode对应一个handler,这个是Handler基类
    public abstract class HandlerBase
    {
        public abstract void OnHandlerMessage(OperationRequest request, OperationResponse response, 
            ClientPeer peer, SendParameters sendParameters);
    }
}
