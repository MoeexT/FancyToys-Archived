using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace FancyServer.Nursery
{
    partial class NurseryManager
    {
        public static void AddProcess(string pathName, string args)
        {
            ProcessManager.Add(pathName, args);
            NoForm.GetTheForm().AddItemToMenu(pathName);
            OperationStruct os = new OperationStruct
            {
                pathName = pathName,
            };
            Send(PDU(os, NurseryCode.OK));
        }
        /// <summary>
        /// NurseryType: Operation
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="args"></param>
        public static void StartProcess(string pathName)
        {
            OperateProcessStruct ops = ProcessManager.Start(pathName);
            NoForm.GetTheForm().SetNurseryItemCheckState(pathName, CheckState.Checked);
            OperationStruct os = new OperationStruct
            {
                pathName = pathName,
                processName = ops.processName
            };
            Send(PDU(os, ops.nurseryCode));
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
            NoForm.GetTheForm().SetNurseryItemCheckState(pathName, CheckState.Unchecked);
            OperationStruct os = new OperationStruct
            {
                remove = true,
                pathName = pathName,
                processName = processName
            };
            Send(PDU(os, NurseryCode.StopUncertain));
        }

        /// <summary>
        /// NurseryType: Operation
        /// 删除进程
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="processName"></param>
        public static void RemoveProcess(string pathName)
        {
            OperateProcessStruct ops = ProcessManager.Remove(pathName);
            NoForm.GetTheForm().RemoveNurseryItem(pathName);
            OperationStruct os = new OperationStruct
            {
                pathName = pathName
            };
            Send(PDU(os, ops.nurseryCode));
        }

        /// <summary>
        /// NurseryType: StandardFile
        /// 标准输出流
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void SendStandardOutput(object sender, DataReceivedEventArgs e)
        {
            StandardFileStruct sfs = new StandardFileStruct
            {
                type = StandardFileType.StandardOutput,
                content = e.Data
            };
            Send(PDU(sfs, NurseryCode.OK));
        }
        /// <summary>
        /// NurseryType: StandardFile
        /// 标准错误流
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void SendStandardError(object sender, DataReceivedEventArgs e)
        {
            StandardFileStruct sfs = new StandardFileStruct
            {
                type = StandardFileType.StandardOutput,
                content = e.Data
            };
            Send(PDU(sfs, NurseryCode.OK));
        }

        private static void Send(NurseryStruct pdu)
        {
            MessageManager.Send(MessageType.nursery, JsonConvert.SerializeObject(pdu));
        }

        private static NurseryStruct PDU(SettingStruct sdu, NurseryCode nc)
        {
            NurseryStruct ns = new NurseryStruct
            {
                type = NurseryType.Setting,
                code = nc,
                content = JsonConvert.SerializeObject(sdu)
            };
            return ns;
        }
        private static NurseryStruct PDU(OperationStruct sdu, NurseryCode nc)
        {
            NurseryStruct ns = new NurseryStruct
            {
                type = NurseryType.Operation,
                code = nc,
                content = JsonConvert.SerializeObject(sdu)
            };
            return ns;
        }
        private static NurseryStruct PDU(List<InformationStruct> sdu, NurseryCode nc)
        {
            NurseryStruct ns = new NurseryStruct
            {
                type = NurseryType.Information,
                code = nc,
                content = JsonConvert.SerializeObject(sdu)
            };
            return ns;
        }
        private static NurseryStruct PDU(StandardFileStruct sdu, NurseryCode nc)
        {
            NurseryStruct ns = new NurseryStruct
            {
                type = NurseryType.StandardFile,
                code = nc,
                content = JsonConvert.SerializeObject(sdu)
            };
            return ns;
        }
        private static NurseryStruct PDU(string ct, NurseryCode nc)
        {
            NurseryStruct ns = new NurseryStruct
            {
                type = NurseryType.Log,
                code = nc,
                content = ct
            };
            return ns;
        }
    }
}
