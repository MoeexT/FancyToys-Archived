using System.Windows.Forms;

using FancyServer.Log;
using FancyServer.Messenger;
using FancyUtil;

namespace FancyServer.Nursery
{
    enum OperationType
    {
        Add = 1,
        Remove = 2,
        Start = 3,
        Stop = 4,
    }
    enum OperationCode
    {
        OK = 1,
        Failed = 2,
    }
    struct OperationStruct
    {
        public OperationType type;
        public OperationCode code;
        public string pathName;     // 要打开的文件
        public string args;         // 参数
        public string processName;  // 打开后的进程名
    }
    class OperationClerk
    {

        public static void Deal(string content)
        {
            bool success = JsonUtil.ParseStruct<OperationStruct>(content, out OperationStruct os);
            if (success)
            {
                switch (os.type)
                {
                    case OperationType.Add:
                        OperationClerk.AddProcess(os.pathName);
                        break;
                    case OperationType.Remove:
                        OperationClerk.RemoveProcess(os.pathName);
                        break;
                    case OperationType.Start:
                        OperationClerk.StartProcess(os.pathName, os.args);
                        break;
                    case OperationType.Stop:
                        OperationClerk.StopProcess(os.pathName);
                        break;
                    default:
                        LogClerk.Warn("Invalid OperationType.");
                        break;
                }
            }
        }
        
        public static void AddProcess(string pathName)
        {
            bool ac = ProcessManager.Add(pathName);
            bool success = NurseryToNoform.AddNurseryItem(pathName);
            NurseryManager.Send(new OperationStruct
            {
                type = OperationType.Add,
                code = ac && success ? OperationCode.OK : OperationCode.Failed,
                pathName = pathName,
            });
        }
        /// <summary>
        /// NurseryType: Operation
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="args"></param>
        public static void StartProcess(string pathName, string args = "")
        {
            string processName = ProcessManager.Start(pathName, args);
            bool uni = NurseryToNoform.UpdateNurseryItem(pathName, processName);
            bool scics = NurseryToNoform.SetNurseryItemCheckState(pathName, CheckState.Checked);
            NurseryManager.Send(new OperationStruct
            {
                type = OperationType.Start,
                code = processName != null && uni && scics ? OperationCode.OK : OperationCode.Failed,
                pathName = pathName,
                processName = processName
            });
        }

        /// <summary>
        /// Operation
        /// 告知前端有进程退出，没有原因（进程退出码的意义不详）
        /// </summary>
        public static void StopProcess(string pathName)
        {
            ProcessManager.Stop(pathName);
        }

        /// <summary>
        /// NurseryType: Operation
        /// 子进程退出时调用
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="processName"></param>
        public static void OnProcessStopped(string pathName, string processName)
        {
            NurseryToNoform.SetNurseryItemCheckState(pathName, CheckState.Unchecked);
            NurseryManager.Send(new OperationStruct
            {
                type = OperationType.Stop,
                code = OperationCode.OK,
                pathName = pathName,
                processName = processName
            });
        }

        /// <summary>
        /// NurseryType: Operation
        /// 删除进程
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="processName"></param>
        public static void RemoveProcess(string pathName)
        {
            ProcessManager.Remove(pathName);
            bool success = NurseryToNoform.RemoveNurseryItem(pathName);
            NurseryManager.Send(new OperationStruct
            {
                type = OperationType.Remove,
                code = success ? OperationCode.OK : OperationCode.Failed,
                pathName = pathName
            });
        }
    }
}
