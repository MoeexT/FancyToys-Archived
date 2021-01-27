using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyToys.Pages.Settings
{
    struct FormSettingStruct
    {
        public int calmSpan;
    }

    struct MessageSettingStruct { }

    struct LoggingSettingStruct
    {
        public int level;
    }

    class SettingsClerk
    {
        public static void SetCalmSpan(int span)
        {
            FormSettingStruct fss = new FormSettingStruct
            {
                calmSpan = span
            };
            SettingsManager.Send(fss);
        }

        public static void SetLogLevel(int level)
        {
            LoggingSettingStruct lss = new LoggingSettingStruct
            {
                level = level
            };
            SettingsManager.Send(lss);
        }
    }
}
