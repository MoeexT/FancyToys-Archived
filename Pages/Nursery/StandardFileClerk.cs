using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace FancyToys.Pages.Nursery
{
    enum StandardFileType
    {
        StandardIn = 0,
        StandardOutput = 1,
        StandardError = 2,
    }
    struct StandardFileStruct
    {
        public StandardFileType type;
        public string processName;
        public string content;
    }

    class StandardFileClerk
    {
        public static void Deal(string message)
        {
            try
            {
                StandardFileStruct sfs = JsonConvert.DeserializeObject<StandardFileStruct>(message);
                switch (sfs.type)
                {
                    case StandardFileType.StandardIn:
                        break;
                    case StandardFileType.StandardOutput:
                        LoggingManager.Info($"{sfs.processName}: {sfs.content}", source: LogSource.Process);
                        break;
                    case StandardFileType.StandardError:
                        LoggingManager.Error($"{sfs.processName}: {sfs.content}", source: LogSource.Process);
                        break;
                    default:
                        LoggingManager.Warn($"Invalid StandardFileType: {sfs.type}");
                        break;
                }
            }
            catch (JsonException e)
            {
                LoggingManager.Warn($"Deserialize NurseryStandardFileStruct failed. {e.Message}");
            }
        }
    }
}
