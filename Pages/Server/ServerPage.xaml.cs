using FancyToys.Pages.Dialog;
using System;
using System.Collections.Generic;
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
    public sealed partial class ServerPage : Page
    {
        private static ServerPage page;

        public static ServerPage Page { get => page; set => page = value; }

        public ServerPage()
        {
            this.InitializeComponent();
            page = this;
        }

        
        public void PrintLog(Color color, string message)
        {
            Paragraph paragraph = new Paragraph
            {
                Foreground = new SolidColorBrush(color),
            };
            Run run = new Run() 
            {
                Text = message
            };
            paragraph.Inlines.Add(run);
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                LogPanel.Blocks.Add(paragraph);
            });
        }

        private void SmallerFontSize(object sender, RoutedEventArgs e)
        {
            LogPanel.FontSize -= 0.5;
        }
        private void LargerFontSize(object sender, RoutedEventArgs e)
        {
            LogPanel.FontSize += 0.5;
        }

        private void LogPanel_Loaded(object sender, RoutedEventArgs e)
        {
            LoggingManager.FlushLogCache();
        }
        
        private void TestLogLevel(object sender, RoutedEventArgs e)
        {
            switch((sender as MenuFlyoutItem).Text)
            {
                case "Trace":
                    LoggingManager.Trace("Trace");
                    break;
                case "Info": 
                    LoggingManager.Info("Info");
                    break;
                case "Debug": 
                    LoggingManager.Debug("Debug");
                    break;
                case "Warn": 
                    LoggingManager.Warn("Warn");
                    break;
                case "Error": 
                    LoggingManager.Error("Error");
                    break;
                case "Fatal": 
                    LoggingManager.Fatal("Fatal");
                    break;
                default:
                    _ = MessageDialog.Error("Error happened while testing log level", "Invalid LogType");
                    break;
            }
        }
    }
}
