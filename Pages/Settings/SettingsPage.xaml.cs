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

using FancyToys.Pages.Dialog;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FancyToys.Pages.Settings
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private HashSet<LogType> LogLevels = new HashSet<LogType>
        {
            LogType.Trace,
            LogType.Info,
            LogType.Debug,
            LogType.Warn,
            LogType.Error,
            LogType.Fatal,
        };
        


        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void ChangeTheme(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is FrameworkElement framework)
            {
                switch ((sender as RadioButton).Content)
                {
                    case "Light":
                        framework.RequestedTheme = ElementTheme.Light;
                        break;
                    case "Dark":
                        framework.RequestedTheme = ElementTheme.Dark;
                        break;
                    case "System":
                        framework.RequestedTheme = ElementTheme.Default;
                        break;
                    default:
                        _ = MessageDialog.Error("Error happened while changing theme", "Invalid theme mode");
                        break;
                }
            }
        }

        private void LogLevelChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox logLevelSelector = sender as ComboBox;
            SettingsClerk.Clerk.SetLogLevel((LogType)logLevelSelector.SelectedValue);

        }
    }
}
