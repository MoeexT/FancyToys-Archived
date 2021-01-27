using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyToys.Pages.Nursery
{

    enum SettingCode
    {
        OK = 1,
        Failed = 2,
    }

    struct SettingStruct
    {
        public SettingCode code;
        public int flushTime;       // 信息刷新时间
    }

    class SettingClerk
    {
        public static void Deal(string message)
        {
            return;
        }

        public static void SetFlushTime(int span)
        {
            NurseryManager.Send(new SettingStruct
            {
                flushTime = span
            });
        }
    }
}
