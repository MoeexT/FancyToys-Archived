using FancyToys.Log.Dialog;
using FancyToys.Log.Nursery;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace FancyToys.Log.Settings
{
    public enum FormSettingType { }
    struct FormSettingStruct { }
    public enum MessageSettingType { }
    struct MessageSettingStruct { }
    public enum LogSettingType
    {
        LogLevel = 1,
        StdLevel = 2,
    }
    public struct LogSettingStruct
    {
        public LogSettingType type;
        public LogType logLevel;
        public StdType stdLevel;
    }
     
    class SettingsClerk
    {
        public static SettingsClerk Clerk { get => clerk; }
        private static readonly SettingsClerk clerk = new SettingsClerk();
        private ApplicationDataContainer lSettings = ApplicationData.Current.LocalSettings;
        public delegate void OpacityChangedHandler();
        public event OpacityChangedHandler OpacityChanged;

        protected virtual void OnOpacityChanged()
        {
            OpacityChanged?.Invoke();
        }

        private class SettingsKeyEnum
        {
            public static string ApplicationTheme = "ApplicationTheme";
            public static string LogPanelOpacity = "LogPanelOpacity";
            public static string LogLevel = "LogLevel";
            public static string StdLevel = "StdLevel";
            public static string FlushTime = "FlushTime";
        }
        public ElementTheme STApplicationTheme
        {
            set
            {
                if (Window.Current.Content is FrameworkElement framework)
                {
                    framework.RequestedTheme = value;
                    lSettings.Values[SettingsKeyEnum.ApplicationTheme] = value.ToString();
                }
            }
            get => LoadEnumSetting<ElementTheme>(SettingsKeyEnum.ApplicationTheme, ElementTheme.Default);
        }

        public double STLogPanelOpacity
        {
            set
            {
                lSettings.Values[SettingsKeyEnum.LogPanelOpacity] = value;
                OnOpacityChanged();
            }
            get => LoadSetting<double>(SettingsKeyEnum.LogPanelOpacity, 0.5);
        }

        public LogType STLogLevel
        {
            set
            {
                lSettings.Values[SettingsKeyEnum.LogLevel] = value.ToString();
                SettingsManager.Send(new LogSettingStruct
                {
                    type = LogSettingType.LogLevel,
                    logLevel = value
                });
            }
            get => LoadEnumSetting<LogType>(SettingsKeyEnum.LogLevel, LogType.Info);
        }

        public StdType STStdLevel
        {
            set
            {
                lSettings.Values[SettingsKeyEnum.StdLevel] = value.ToString();
                SettingsManager.Send(new LogSettingStruct
                {
                    type = LogSettingType.StdLevel,
                    stdLevel = value
                });
            }
            get => LoadEnumSetting<StdType>(SettingsKeyEnum.StdLevel, StdType.Error);
        }

        public int STFlushTime
        {
            set
            {
                lSettings.Values[SettingsKeyEnum.FlushTime] = value;
                ConfigClerk.SetFlushTime(value);
            }
            get => LoadSetting<int>(SettingsKeyEnum.FlushTime, 1000);
        }


        private SettingsClerk() { }

        public void InitlailzeLocalSettings()
        {
            STFlushTime = 1000;
            STLogLevel = LoadEnumSetting<LogType>(SettingsKeyEnum.LogLevel, LogType.Info);
            STStdLevel = LoadEnumSetting<StdType>(SettingsKeyEnum.StdLevel, StdType.Error);
            STLogPanelOpacity = LoadSetting<double>(SettingsKeyEnum.LogPanelOpacity, 0.55);
            STApplicationTheme = LoadEnumSetting<ElementTheme>(SettingsKeyEnum.ApplicationTheme, ElementTheme.Default);
        }


        private T LoadSetting<T>(string sKey, object dValue)
        {
            if (lSettings.Values.TryGetValue(sKey, out object value))
            {
                return (T)value;
            }
            else
            {
                LoggingManager.Info($"Load default {sKey} setting: {(T)dValue}");
                return (T)dValue;
            }
        }

        private T LoadEnumSetting<T>(string sKey, object dValue)
        {
            if (lSettings.Values.TryGetValue(sKey, out object value))
            {
                /* A TERRIBLE BUG !!! 
                 * After uncomment this line, the program whill fall into endless loop.
                 * I tried to find the reason but failed. 
                LoggingManager.Debug($"Load setting: {(value as string)}");*/
                return ParseEnum<T>(value as string);
            }
            else
            {
                LoggingManager.Info($"Load default {sKey} setting: {(T)dValue}");
                return (T)dValue;
            }
        }

        private T ParseEnum<T>(object value)
        {
            if (value == null)
            {
                return default;
            }
            return (T)Enum.Parse(typeof(T), value.ToString());
        }
        
    }
}
