using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

using FancyServer.Bridge;
using System;

namespace FancyServer.Nursery
{
    partial class NurseryManager
    {
        public static void AddProcess(string pathName, string args)
        {
            ProcessManager.Add(pathName, args);
            NurseryToNoform.AddNurseryItem(pathName);
            OperationStruct os = new OperationStruct
            {
                pathName = pathName,
            };
            Send(os, NurseryCode.OK);
        }
        /// <summary>
        /// NurseryType: Operation
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="args"></param>
        public static void StartProcess(string pathName)
        {
            OperateProcessStruct ops = ProcessManager.Start(pathName);
            NurseryToNoform.SetNurseryItemCheckState(pathName, CheckState.Checked);
            OperationStruct os = new OperationStruct
            {
                pathName = pathName,
                processName = ops.processName
            };
            Send(os, ops.nurseryCode);
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
            OperationStruct os = new OperationStruct
            {
                remove = true,
                pathName = pathName,
                processName = processName
            };
            Send(os, NurseryCode.StopUncertain);
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
            NurseryToNoform.RemoveNurseryItem(pathName);
            OperationStruct os = new OperationStruct
            {
                pathName = pathName
            };
            Send(os, ops.nurseryCode);
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
            Send(sfs, NurseryCode.OK);
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
            Send(sfs, NurseryCode.OK);
        }

        private static void Send(object sdu, NurseryCode nc)
        {
            NurseryStruct? pdu = null;
            switch (sdu)
            {
                case SettingStruct ss:
                    pdu = PDU(NurseryType.Setting, nc, JsonConvert.SerializeObject(ss));
                    break;
                case OperationStruct os:
                    pdu = PDU(NurseryType.Setting, nc, JsonConvert.SerializeObject(os));
                    break;
                case List<InformationStruct> lis:
                    pdu = PDU(NurseryType.Setting, nc, JsonConvert.SerializeObject(lis));
                    break;
                case StandardFileStruct sfs:
                    pdu = PDU(NurseryType.Setting, nc, JsonConvert.SerializeObject(sfs));
                    break;
                default:
                    LoggingManager.Error("Invalid nursery SDU type.", 2);
                    break;
            }
            if (pdu != null)
            {
                MessageManager.Send(pdu);
            }
        }

        private static NurseryStruct PDU(NurseryType nt, NurseryCode nc, string sdu)
        {
            NurseryStruct ns = new NurseryStruct
            {
                type = NurseryType.Setting,
                code = nc,
                content = sdu
            };
            return ns;
        }
    }
}
