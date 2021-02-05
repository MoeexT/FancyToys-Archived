using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;

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
                List<InformationStruct> ins = JsonConvert.DeserializeObject<List<InformationStruct>>(message);
                ins.Sort((x, y) => -x.pid.CompareTo(y.pid));
                NurseryPage.Page.UpdateProcessInformation(ins);
            }
            catch (JsonException e)
            {
                LoggingManager.Warn($"Deserialize OperationStruct failed. {e.Message}");
            }
        }
    }
}
