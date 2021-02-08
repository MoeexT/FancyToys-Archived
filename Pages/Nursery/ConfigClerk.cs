using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyToys.Log.Nursery
{
    enum ConfigType
    {
        FlushTime = 1,
    }

    struct ConfigStruct
    {
        public ConfigType type;
        public int flushTime;       // 信息刷新时间
    }

    class ConfigClerk
    {
        public static void Deal(string message)
        {
            return;
        }

        public static void SetFlushTime(int span)
        {
            NurseryManager.Send(new ConfigStruct
            {
                type = ConfigType.FlushTime,
                flushTime = span
            });
        }
    }
}
