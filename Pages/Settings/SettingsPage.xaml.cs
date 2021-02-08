using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using FancyToys.Log.Dialog;
using System.ComponentModel;
using System.Diagnostics;
using FancyToys.Log.Nursery;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FancyToys.Log.Settings
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        public static SettingsPage Page { private set; get; }
        public double OpacitySliderValue {
            get => SettingsClerk.Clerk.STLogPanelOpacity;
            set
            {
                SettingsClerk.Clerk.STLogPanelOpacity = value;
                RaisePropertyChanged(nameof(OpacitySliderValue));
            }
        }

        private List<ComboBoxItem> LogLevelList
        {
            get
            {
                Array levelArr = Enum.GetValues(typeof(LogType));
                List<ComboBoxItem> levels = new List<ComboBoxItem>();
                foreach(LogType level in levelArr)
                {
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = level,
                        Foreground = new SolidColorBrush(LoggingManager.LogForegroundColors[level]),
                    };
                    levels.Add(item);
                }
                return levels;
            }
        }

        private List<ComboBoxItem> StdLevelList
        {
            get
            {
                Array levelArr = Enum.GetValues(typeof(StdType));
                List<ComboBoxItem> levels = new List<ComboBoxItem>();
                foreach(StdType level in levelArr)
                {
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = level,
                        Foreground = new SolidColorBrush(LoggingManager.StdForegroundColors[level])
                    };
                    levels.Add(item);
                }
                return levels;
            }
        }


        public SettingsPage()
        {
            this.InitializeComponent();
            Page = this;
            InitializeControls();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        private void ChangeTheme(object sender, RoutedEventArgs e)
        {
            switch ((sender as RadioButton).Content)
            {
                case "Light":
                    SettingsClerk.Clerk.STApplicationTheme = ElementTheme.Light;
                    break;
                case "Dark":
                    SettingsClerk.Clerk.STApplicationTheme = ElementTheme.Dark;
                    break;
                case "System":
                    SettingsClerk.Clerk.STApplicationTheme = ElementTheme.Default;
                    break;
                default:
                    _ = MessageDialog.Error("Error happened while changing theme", "Invalid theme mode");
                    break;
            }
        }

        private void LogLevelChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cs = sender as ComboBox;
            var item = cs.SelectedValue as ComboBoxItem;
            var eb = (cs.Header as TextBlock).Foreground;
            cs.Foreground = item.Foreground;
            (cs.Header as TextBlock).Foreground = eb;
            SettingsClerk.Clerk.STLogLevel = (LogType)item.Content;
        }

        private void StdLevelChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cs = sender as ComboBox;
            var item = cs.SelectedValue as ComboBoxItem;
            var eb = (cs.Header as TextBlock).Foreground;
            cs.Foreground = item.Foreground;
            (cs.Header as TextBlock).Foreground = eb;
            SettingsClerk.Clerk.STStdLevel = (StdType)item.Content;
        }

        private void OpatitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SettingsClerk.Clerk.STLogPanelOpacity = (sender as Slider).Value;
        }

        private int IndexOfLogLevels()
        {
            for (int i = 0; i < LogLevelList.Count; i++)
            {
                if ((LogType)LogLevelList[i].Content == SettingsClerk.Clerk.STLogLevel) {
                    return i;
                }
            }
            LoggingManager.Warn("Can't find right log level.");
            return 0;
        }
        
        private int IndexOfStdLevels()
        {
            for (int i = 0; i < StdLevelList.Count; i++)
            {
                if ((StdType)StdLevelList[i].Content == SettingsClerk.Clerk.STStdLevel) {
                    return i;
                }
            }
            LoggingManager.Warn("Can't find right std level.");
            return 0;
        }

        private void InitializeControls()
        {
            // set app theme
            switch (SettingsClerk.Clerk.STApplicationTheme)
            {
                case ElementTheme.Light:
                    LightButton.IsChecked = true;
                    break;
                case ElementTheme.Dark:
                    DarkButton.IsChecked = true;
                    break;
                case ElementTheme.Default:
                    SystemButton.IsChecked = true;
                    break;
                default: break;
            }
        }

        
    }
}
