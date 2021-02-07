using FancyToys.Pages.Dialog;
using FancyToys.Pages.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FancyToys.Pages.Server
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ServerPage : Page, INotifyPropertyChanged
    {
        public static ServerPage Page { get => page; set => page = value; }
        private static ServerPage page;

        private double logPanelOpacity = SettingsClerk.Clerk.STLogPanelOpacity;
        private double LogPanelOpacity {
            get => logPanelOpacity;
            set
            {
                logPanelOpacity = value;
                RaisePropertyChanged(nameof(LogPanelOpacity));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ServerPage()
        {
            this.InitializeComponent();
            page = this;
        }

        
        public void PrintLog(string source, string content, Color foreground, Color background=default)
        {
            Paragraph paragraph = new Paragraph();
            Run src = new Run() 
            {
                Foreground = new SolidColorBrush(foreground),
                Text = $"{source}> ",
            };
            Run msg = new Run()
            {
                Text = content,
                Foreground = new SolidColorBrush(Colors.White),
            };
            paragraph.Inlines.Add(src);
            paragraph.Inlines.Add(msg);
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                LogPanel.Blocks.Add(paragraph);
            });
        }

        private void LogPanel_Loaded(object sender, RoutedEventArgs e)
        {
            LoggingManager.FlushLogCache();
            SettingsClerk.Clerk.OpacityChanged += () =>
            {
                LogPanelOpacity = SettingsClerk.Clerk.STLogPanelOpacity;
            };
        }

        private void SmallerFontSize(object sender, RoutedEventArgs e)
        {
            LogPanel.FontSize -= 0.5;
        }
        private void LargerFontSize(object sender, RoutedEventArgs e)
        {
            LogPanel.FontSize += 0.5;
        }
        
        private void TestLogLevel(object sender, RoutedEventArgs e)
        {
            switch((sender as MenuFlyoutItem).Text)
            {
                case "Output":
                    LoggingManager.StandardOutput("puppet", "output");
                    break;
                case "Error": 
                    LoggingManager.StandardError("puppet", "error");
                    break;
                default:
                    _ = MessageDialog.Error("Error happened while testing log level", "Invalid LogType");
                    break;
            }
        }

        private void ClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            LogPanel.Blocks.Clear();
        }

        private void ShowStdLevel_Click(object sender, RoutedEventArgs e)
        {
            PrintLog($"{LogSource.FancyToys}", $"{SettingsClerk.Clerk.STStdLevel}", Colors.Azure);
        }
    }
}
