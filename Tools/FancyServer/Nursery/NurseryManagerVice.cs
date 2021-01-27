using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

using Newtonsoft.Json;

using FancyServer.Messenger;

namespace FancyServer.Nursery
{
    partial class NurseryManager
    {
        public static void AddProcess(string pathName)
        {
            bool add = ProcessManager.Add(pathName);
            bool success = NurseryToNoform.AddNurseryItem(pathName);
            OperationStruct os = new OperationStruct
            {
                type = OperationType.Add,
                code = add&&success ? OperationCode.OK : OperationCode.Failed,
                pathName = pathName,
            };
            Send(os);
        }
        /// <summary>
        /// NurseryType: Operation
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="args"></param>
        public static void StartProcess(string pathName, string args="")
        {
            string processName = ProcessManager.Start(pathName, args);
            NurseryToNoform.UpdateNurseryItem(pathName, processName);
            bool success = NurseryToNoform.SetNurseryItemCheckState(pathName, CheckState.Checked);
            OperationStruct os = new OperationStruct
            {
                type = OperationType.Start,
                code = processName != null && success ? OperationCode.OK : OperationCode.Failed,
                pathName = pathName,
                processName = processName
            };
            Send(os);
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
            bool success = NurseryToNoform.SetNurseryItemCheckState(pathName, CheckState.Unchecked);
            Send(new OperationStruct
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
            NurseryToNoform.RemoveNurseryItem(pathName);
            //Send(new OperationStruct
            //{
            //    type = OperationType.Remove,
            //    code = success ? OperationCode.OK : OperationCode.Failed,
            //    pathName = pathName
            //});
        }

        /// <summary>
        /// NurseryType: StandardFile
        /// 标准输出流
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void SendStandardOutput(object sender, DataReceivedEventArgs e)
        {
            Process s = sender as Process;
            Send(new StandardFileStruct
            {
                type = StandardFileType.StandardOutput,
                processName = s.ProcessName,
                content = e.Data
            });
        }

        /// <summary>
        /// NurseryType: StandardFile
        /// 标准错误流
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void SendStandardError(object sender, DataReceivedEventArgs e)
        {
            Process s = sender as Process;
            Send(new StandardFileStruct
            {
                type = StandardFileType.StandardError,
                processName = s.ProcessName,
                content = e.Data
            });
        }

        private static void Send(object sdu)
        {
            NurseryStruct? pdu = null;
            switch (sdu)
            {
                case SettingStruct ss:
                    pdu = PDU(NurseryType.Setting, JsonConvert.SerializeObject(ss));
                    break;
                case OperationStruct os:
                    pdu = PDU(NurseryType.Operation, JsonConvert.SerializeObject(os));
                    break;
                case List<InformationStruct> lis:
                    pdu = PDU(NurseryType.Information, JsonConvert.SerializeObject(lis));
                    break;
                case StandardFileStruct sfs:
                    pdu = PDU(NurseryType.StandardFile, JsonConvert.SerializeObject(sfs));
                    break;
                default:
                    LoggingManager.Warn("Invalid nursery SDU type.", 2);
                    break;
            }
            if (pdu != null)
            {
                MessageManager.Send(pdu);
            }
        }

        private static NurseryStruct PDU(NurseryType nt, string sdu)
        {
            NurseryStruct ns = new NurseryStruct
            {
                type = NurseryType.Setting,
                content = sdu
            };
            return ns;
        }
    }
}
