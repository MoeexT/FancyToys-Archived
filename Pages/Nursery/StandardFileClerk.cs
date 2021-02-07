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
        //StandardIn = 0,
        Output = 1,
        Error = 2,
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
                    case StandardFileType.Output:
                        if (sfs.content.Trim().Length != 0)
                        {
                            LoggingManager.StandardOutput(sfs.processName, sfs.content);
                        }
                        break;
                    case StandardFileType.Error:
                        if (sfs.content.Trim().Length != 0)
                        {
                            LoggingManager.StandardError(sfs.processName, sfs.content);
                        }
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
