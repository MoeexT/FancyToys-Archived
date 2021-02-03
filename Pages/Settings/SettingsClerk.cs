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
        public static SettingsClerk Clerk { get => clerk; }
        private static readonly SettingsClerk clerk = new SettingsClerk();
        private ApplicationDataContainer LSettings = ApplicationData.Current.LocalSettings;
        public delegate void OpacityChangedHandler();
        public event OpacityChangedHandler OpacityChanged;

        protected virtual void OnOpacityChanged()
        {
            OpacityChanged?.Invoke();
        }

        public LogType STLogLevel
        {
            set
            {
                LSettings.Values["LogLevel"] = value.ToString();
            }
            get
            {
                if (LSettings.Values.TryGetValue("LogLevel", out object val))
                {
                    return ParseEnum<LogType>(val as string);
                }
                _ = MessageDialog.Error("Error while setting Log Level", "Setting not found. Default log level `Info` setted.");
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

        public double STLogPanelOpacity
        {
            set
            {
                LSettings.Values["LogPanelOpacity"] = value;
                OnOpacityChanged();
            }
            get
            {
                if (LSettings.Values.TryGetValue("LogPanelOpacity", out object opacity))
                {
                    return (double)opacity;
                }
                _ = MessageDialog.Error("Error while setting Log Panel Opacity", "Setting not found. Default opacity `0.5` setted.");
                return 0.5;
            }
        }


        private SettingsClerk() { }

        public void InitlailzeLocalSettings()
        {
            STLogLevel = LogType.Trace;
            STLogPanelOpacity = 0.5;
        }

        public void SetLogLevel(LogType level)
        {
            STLogLevel = level;

            LoggingSettingStruct lss = new LoggingSettingStruct
            {
                level = level
            };
            SettingsManager.Send(lss);
        }
    }
}
