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
    struct FormSettingStruct { }
    struct MessageSettingStruct { }

    struct LoggingSettingStruct
    {
        public LogLevel level;
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

        public LogLevel STLogLevel
        {
            set => lSettings.Values[SettingsKeyEnum.LogLevel] = value.ToString();
            get => LoadEnumSetting<LogLevel>(SettingsKeyEnum.LogLevel, LogLevel.Info);
        }

        public StandardFileType STStdLevel
        {
            set => lSettings.Values[SettingsKeyEnum.StdLevel] = value.ToString();
            get => LoadEnumSetting<StandardFileType>(SettingsKeyEnum.StdLevel, StandardFileType.Error);
        }


        private SettingsClerk() { }

        public void InitlailzeLocalSettings()
        {
            STLogLevel = LoadEnumSetting<LogLevel>(SettingsKeyEnum.LogLevel, LogLevel.Info);
            STStdLevel = LoadEnumSetting<StandardFileType>(SettingsKeyEnum.StdLevel, StandardFileType.Error);
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
