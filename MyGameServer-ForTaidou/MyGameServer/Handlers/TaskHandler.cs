using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using MyGameServer.DataBase.Manager;
using MyGameCommon.Model;
using MyGameCommon;
using ExitGames.Logging;
using MyGameServer;
using MyGameServer.Tool;
using MyGameServer.DataBase.Model;

namespace MyGameServer.Handlers
{
    public class TaskHandler : HandlerBase
    {
        private TaskDataMgr m_mgr;
        public TaskHandler()
        {
            m_mgr = new TaskDataMgr();
        }
        public override void OnHandlerMessage(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            byte operCode = request.OperationCode;
            switch (operCode)
            {
                case (byte)OperationCode.GetTask:
                    GetTaskList(request, response, peer, sendParameters);
                    break;
                case (byte)OperationCode.AddTask:
                    AddTask(request, response, peer, sendParameters);
                    break;
                case (byte)OperationCode.UpdateTask:
                    UpdateTask(request, response, peer, sendParameters);
                    break;
            }
        }

        public void GetTaskList(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Helper.Log("Task Handler GetTaskList");
            if (peer.m_curRole == null)
            {
                response.ReturnCode = (short)ReturnCode.Fail;
                response.DebugMessage = "GetTaskList:当前角色为空";
                return;
            }

            response.ReturnCode = (short)ReturnCode.Fail;
            // 当前的角色信息保存在peer中，直接可以从peer获取
            List<TaskData> pTaskList = m_mgr.GetTaskList(peer.m_curRole.Id);
      
            List<int> pTaskID = new List<int>();
            if (pTaskList != null)
            {
                string strRes = "";
                for (int i = 0; i < pTaskList.Count; ++i)
                {
                    TaskData curTask = pTaskList[i];
                    if (!pTaskID.Contains(curTask.TaskID))
                    {
                        strRes += curTask.TaskID + "," + curTask.TaskPro + "|";
                        pTaskID.Add(curTask.TaskID);
                    }
                }
                Helper.Log("GetTaskList Return:" + strRes);
                Dictionary<byte, object> m_dic = new Dictionary<byte, object>();
                m_dic.Add((byte)ParameterCode.TaskInfo, strRes);
                response.Parameters = m_dic;
                response.ReturnCode = (short)ReturnCode.Success;
            }
        }

        public void AddTask(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            // 客户端传递过来的参数信息：TaskID,TaskType,TaskPro,DataTime|
            Dictionary<byte, object> m_dic = request.Parameters;
            object value;
            response.ReturnCode = (short)ReturnCode.Fail;
            if (m_dic.TryGetValue((byte)ParameterCode.TaskInfo, out value))
            {
                string strValue = value.ToString();
                Helper.Log("AddTask:" + strValue);
                string[] pStr = strValue.Split('|');
                for (int i = 0; i < pStr.Length; ++i)
                {
                    string strTaskItem = pStr[i];
                    string[] pStr1 = strTaskItem.Split(',');
                    if (pStr1.Length == 3)
                    {
                        TaskData newTask = new TaskData();
                        newTask.TaskID = Helper.IntParse(pStr1[0]);
                        newTask.TaskType = Helper.IntParse(pStr1[1]);
                        newTask.TaskPro = Helper.IntParse(pStr1[2]);
                        newTask.LastupdateTime = DateTime.Now.ToShortDateString();
                        newTask.Role = peer.m_curRole;
                        m_mgr.AddTask(newTask);
                        Helper.Log("Add One New Task:" + newTask.TaskID);
                        response.ReturnCode = (short)ReturnCode.Success;
                    }
                }
            }
        }

        public void UpdateTask(OperationRequest request, OperationResponse response, ClientPeer peer, SendParameters sendParameters)
        {
            Dictionary<byte, object> m_dic = request.Parameters;
            object value;
            response.ReturnCode = (short)ReturnCode.Fail;
            // 客户端传递过来的参数信息：TaskID,TaskPro,DataTime|
            if (m_dic.TryGetValue((byte)ParameterCode.TaskInfo, out value))
            {
                string strValue = value.ToString();
                Helper.Log("UpdateTask:" + strValue);
                string[] pStr1 = strValue.Split(',');
                if (pStr1.Length == 2)
                {
                    int nTaskID = Helper.IntParse(pStr1[0]);
                    List<TaskData> pTask = m_mgr.GetTaskByTaskID(nTaskID);
                    if (pTask != null && pTask.Count > 0)
                    {
                        TaskData newTask = pTask[0];
                        newTask.TaskPro = Helper.IntParse(pStr1[1]);
                        newTask.LastupdateTime = DateTime.Now.ToShortDateString();
                        m_mgr.UpdateTask(newTask);
                        Helper.Log("Update One Task:" + newTask.TaskID);
                        response.ReturnCode = (short)ReturnCode.Success;
                    }
                }
            }
        }
    }
}
