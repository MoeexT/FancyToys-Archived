using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                            Add(os.pathName);
                        }
                        break;
                    case OperationType.Remove:
                        break;
                    case OperationType.Start:
                        if (os.code == OperationCode.OK)
                        {
                            Start(os.pathName, os.processName);
                        }
                        break;
                    case OperationType.Stop:
                        Stop(os.pathName);
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

        private static void Add(string pathName)
        {
            // 拿到NurseryPage对象，调用
            NurseryPage.GetThis().AddSwitch(pathName);

        }

        private static void Start(string pathName, string processName)
        {
            NurseryPage.GetThis().UpdateSwitch(pathName, processName);
        }

        private static void Stop(string pathName)
        {
            recentStop.Add(pathName);
            NurseryPage.GetThis().TogglSwitch(pathName, false);
        }

        [Obsolete]
        private static void Remove(string pathName) { return; }

        public static void AddProcess(string pathName)
        {
            NurseryManager.Send(new OperationStruct
            {
                type = OperationType.Add,
                pathName = pathName
            });
        }

        public static void RemoveProcess(string pathName)
        {
            NurseryManager.Send(new OperationStruct
            {
                type = OperationType.Remove,
                pathName = pathName
            });
        }

        public static void StartProcess(string pathName, string args)
        {
            NurseryManager.Send(new OperationStruct
            {
                type = OperationType.Start,
                pathName = pathName,
                args = args
            });
        }

        public static void StopProcess(string pathName)
        {
            if (!recentStop.Contains(pathName))
            {
                NurseryManager.Send(new OperationStruct
                {
                    type = OperationType.Stop,
                    pathName = pathName
                });
            }
            else
            {
                recentStop.Remove(pathName);
            }
        }
    }
}
