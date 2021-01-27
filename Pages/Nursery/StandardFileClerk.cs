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
        private static Color outputColor = Colors.Aqua;
        private static Color errorColor = Colors.Red;

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
                        Print(outputColor, sfs.content);
                        break;
                    case StandardFileType.StandardError:
                        Print(errorColor, sfs.content);
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

        private static void Print(Color color, string message)
        {

        }

    }
}
