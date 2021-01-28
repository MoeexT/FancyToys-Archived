using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace FancyToys.Pages.Nursery
{
    enum OperationCode
    {
        OK = 1,
        Failed = 2,
    }

    enum OperationType
    {
        Add = 1,
        Remove = 2,
        Start = 3,
        Stop = 4,
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

        private static HashSet<string> recentStop = new HashSet<string>();

        public static void Deal(string message)
        {
            try
            {
                OperationStruct os = JsonConvert.DeserializeObject<OperationStruct>(message);
                switch (os.type)
                {
                    case OperationType.Add:
                        if (os.code == OperationCode.OK)
                        {
                            ConfirmAdd(os.pathName);
                        }
                        break;
                    case OperationType.Remove:
                        if (os.code == OperationCode.OK)
                        {
                            ConfirmRemove(os.pathName);
                        }
                        break;
                    case OperationType.Start:
                        if (os.code == OperationCode.OK)
                        {
                            ConfirmStart(os.pathName, os.processName);
                        }
                        break;
                    case OperationType.Stop:
                        ConfirmStop(os.pathName);
                        break;
                    default:
                        LoggingManager.Warn("Invalid OperationType.");
                        break;
                }
            }
            catch (JsonException e)
            {
                LoggingManager.Warn($"Deserialize NurseryOperationStruct failed. {e.Message}");
            }
        }

        private static void ConfirmAdd(string pathName)
        {
            NurseryPage.Page.AddSwitch(pathName);
        }

        private static void ConfirmStart(string pathName, string processName)
        {
            NurseryPage.Page.UpdateSwitch(pathName, processName);
        }

        private static void ConfirmStop(string pathName)
        {
            NurseryPage.Page.TogglSwitch(pathName, false);
        }

        private static void ConfirmRemove(string pathName) 
        {
            NurseryPage.Page.RemoveSwitch(pathName);
        }

        public static void TryAdd(string pathName)
        {
            NurseryManager.Send(new OperationStruct
            {
                type = OperationType.Add,
                pathName = pathName
            });
        }

        public static void TryRemove(string pathName)
        {
            NurseryManager.Send(new OperationStruct
            {
                type = OperationType.Remove,
                pathName = pathName
            });
        }

        public static void TryStart(string pathName, string args)
        {
            NurseryManager.Send(new OperationStruct
            {
                type = OperationType.Start,
                pathName = pathName,
                args = args
            });
        }

        public static void TryStop(string pathName)
        {
            NurseryManager.Send(new OperationStruct
            {
                type = OperationType.Stop,
                pathName = pathName
            });
        }
    }
}
