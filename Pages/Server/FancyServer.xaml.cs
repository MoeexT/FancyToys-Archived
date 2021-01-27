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
    public sealed partial class FancyServer : Page
    {
        public static FancyServer sp;
        public static FancyServer GetThis() { return sp; }
        public FancyServer()
        {
            this.InitializeComponent();
            sp = this;
        }

        
        public void UpdateLog(Color color, string message)
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
            LogPanel.FontSize--;
        }
        private void LargerFontSize(object sender, RoutedEventArgs e)
        {
            LogPanel.FontSize++;
        }

        private void CopySelectedLog(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_Opening(object sender, object e)
        {
            CommandBarFlyout thisFlyout = sender as CommandBarFlyout;
            if (thisFlyout.Target == LogPanel)
            {
                
            }
        }


        private void LogPanel_Loaded(object sender, RoutedEventArgs e)
        {
            LoggingManager.Warn($"{LogPanel.SelectionFlyout == null}");
            LogPanel.SelectionFlyout.Opening += Menu_Opening;
            LogPanel.ContextFlyout.Opening += Menu_Opening;
        }

        private void LogPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            LogPanel.SelectionFlyout.Opening -= Menu_Opening;
            LogPanel.ContextFlyout.Opening -= Menu_Opening;
        }
    }
}