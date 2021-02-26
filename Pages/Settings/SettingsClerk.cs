using FancyToys.Log;
using FancyToys.Pages.Dialog;
using FancyToys.Pages.Nursery;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace FancyToys.Pages.Settings
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

        private class SettingKeys
        {
            public static string AppTheme = "ApplicationTheme";
            public static string LogOpacity = "LogPanelOpacity";
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
                    lSettings.Values[SettingKeys.AppTheme] = value.ToString();
                }
            }
            get => LoadEnumSetting<ElementTheme>(SettingKeys.AppTheme, ElementTheme.Default);
        }

        public double STLogOpacity
        {
            set
            {
                lSettings.Values[SettingKeys.LogOpacity] = value;
                OnOpacityChanged();
            }
            get => LoadSetting<double>(SettingKeys.LogOpacity, 0.5);
        }

        public LogType STLogLevel
        {
            set
            {
                lSettings.Values[SettingKeys.LogLevel] = value.ToString();
                SettingsManager.Send(new LogSettingStruct
                {
                    type = LogSettingType.LogLevel,
                    logLevel = value
                });
            }
            get => LoadEnumSetting<LogType>(SettingKeys.LogLevel, LogType.Info);
        }

        public StdType STStdLevel
        {
            set
            {
                lSettings.Values[SettingKeys.StdLevel] = value.ToString();
                SettingsManager.Send(new LogSettingStruct
                {
                    type = LogSettingType.StdLevel,
                    stdLevel = value
                });
            }
            get => LoadEnumSetting<StdType>(SettingKeys.StdLevel, StdType.Error);
        }

        public int STFlushTime
        {
            set
            {
                lSettings.Values[SettingKeys.FlushTime] = value;
                ConfigClerk.SetFlushTime(value);
            }
            get => LoadSetting<int>(SettingKeys.FlushTime, 1000);
        }


        private SettingsClerk() { }

        public void InitlailzeLocalSettings()
        {
            STFlushTime = LoadSetting<int>(SettingKeys.FlushTime, 1000);
            STLogOpacity = LoadSetting<double>(SettingKeys.LogOpacity, 0.55);
            STLogLevel = LoadEnumSetting<LogType>(SettingKeys.LogLevel, LogType.Info);
            STStdLevel = LoadEnumSetting<StdType>(SettingKeys.StdLevel, StdType.Error);
            STApplicationTheme = LoadEnumSetting<ElementTheme>(SettingKeys.AppTheme, ElementTheme.Default);
        }


        private T LoadSetting<T>(string sKey, object dValue)
        {
            if (lSettings.Values.TryGetValue(sKey, out object value))
            {
                Debug.WriteLine($"got: {(T)value}");
                return (T)value;
            }
            else
            {
                Debug.WriteLine($"Load default {sKey} setting: {(T)dValue}");
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
                Debug.WriteLine($"got: {ParseEnum<T>(value as string)}");
                return ParseEnum<T>(value as string);
            }
            else
            {
                Debug.WriteLine($"Load default {sKey} setting: {dValue}");
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
