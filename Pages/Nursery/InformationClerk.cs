using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;

using FancyToys.Log;

namespace FancyToys.Pages.Nursery
{
    public struct InformationStruct
    {
        public int pid;
        public string processName;
        public float cpu;
        public int memory;
    }
    class InformationClerk
    {
        public static void Deal(string message)
        {
            try
            {
                Dictionary<int, InformationStruct> ins = JsonConvert.DeserializeObject<Dictionary<int, InformationStruct>>(message);
                NurseryPage.Page.UpdateProcessInformation(ins);
            }
            catch (JsonException e)
            {
                LoggingManager.Warn($"Deserialize OperationStruct failed. {e.Message}");
            }
        }
    }
}
