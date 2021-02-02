using FancyToys.Pages.Dialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FancyToys.Pages.Settings
{
    struct FormSettingStruct { }
    struct MessageSettingStruct { }

    struct LoggingSettingStruct
    {
        public LogType level;
    }

    class SettingsClerk
    {
        private static readonly SettingsClerk clerk = new SettingsClerk();
        public static SettingsClerk Clerk { get => clerk; }
        public LogType LogLevel
        {
            set
            {
                ApplicationData.Current.LocalSettings.Values["LogLevel"] = value.ToString();
            }
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.TryGetValue("LogLevel", out object val))
                {
                    return ParseEnum<LogType>(val as string);
                }
                _ = MessageDialog.Error("Error while setting Log Level", "Setting not found.");
                return LogType.Info;
            }
        }
        T ParseEnum<T>(object value)
        {
            if (value == null)
            {
                return default;
            }
            return (T)Enum.Parse(typeof(T), value.ToString());
        }


        private SettingsClerk() { }

        public void InitlailzeLocalSettings()
        {
            LogLevel = LogType.Trace;
        }

        public void SetLogLevel(LogType level)
        {
            LogLevel = level;
            LoggingManager.Debug($"{LogLevel}");

            LoggingSettingStruct lss = new LoggingSettingStruct
            {
                level = level
            };
            SettingsManager.Send(lss);
        }
    }
}
