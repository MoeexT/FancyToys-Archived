using System.Windows.Forms;

using FancyUtil;

using FancyServer.Logging;
using FancyServer.Messenger;


namespace FancyServer.Nursery {

    internal enum OperationType {
        Add = 1,
        Remove = 2,
        Start = 3,
        Stop = 4,
    }

    internal enum OperationCode {
        OK = 1,
        Failed = 2,
    }

    internal struct OperationStruct {
        public OperationType type;
        public OperationCode code;
        public string pathName; // 要打开的文件
        public string args; // 参数
        public string processName; // 打开后的进程名
    }

    internal static class OperationClerk {
        public static void Deal(string content) {
            bool success = JsonUtil.ParseStruct(content, out OperationStruct os);

            if (!success) return;

            switch (os.type) {
                case OperationType.Add:
                    AddProcess(os.pathName);
                    break;
                case OperationType.Remove:
                    RemoveProcess(os.pathName);
                    break;
                case OperationType.Start:
                    StartProcess(os.pathName, os.args);
                    break;
                case OperationType.Stop:
                    StopProcess(os.pathName);
                    break;
                default:
                    LogClerk.Warn("Invalid OperationType.");
                    break;
            }
        }

        public static void AddProcess(string pathName) {
            bool ac = ProcessManager.Add(pathName);
            bool success = NurseryToNoform.AddNurseryItem(pathName);

            NurseryManager.Send(new OperationStruct {
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
        public static void StartProcess(string pathName, string args = "") {
            string processName = ProcessManager.Start(pathName, args);
            bool uni = NurseryToNoform.UpdateNurseryItem(pathName, processName);
            bool scics = NurseryToNoform.SetNurseryItemCheckState(pathName, CheckState.Checked);

            NurseryManager.Send(new OperationStruct {
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
        public static void StopProcess(string pathName) { ProcessManager.Stop(pathName); }

        /// <summary>
        /// NurseryType: Operation
        /// 子进程退出时调用
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="processName"></param>
        public static void OnProcessStopped(string pathName, string processName) {
            NurseryToNoform.SetNurseryItemCheckState(pathName, CheckState.Unchecked);

            NurseryManager.Send(new OperationStruct {
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
        /// <param name="pathName">the full name of process</param>
        private static void RemoveProcess(string pathName) {
            ProcessManager.Remove(pathName);
            bool success = NurseryToNoform.RemoveNurseryItem(pathName);

            NurseryManager.Send(new OperationStruct {
                type = OperationType.Remove,
                code = success ? OperationCode.OK : OperationCode.Failed,
                pathName = pathName
            });
        }
    }

}