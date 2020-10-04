using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace FancyToys.Pages.Dialog
{
    public sealed partial class MessageDialog : ContentDialog
    {
        public MessageDialog()
        {
            this.InitializeComponent();
        }
        public MessageDialog(string message)
        {
            this.InitializeComponent();
            TheTextBlock.Text = message;
        }
        public MessageDialog(string title, string message)
        {
            this.InitializeComponent();
            Title = title;
            TheTextBlock.Text = message;
        }

        public void SetButtonText(string primaryMessage, string secondaryMessage)
        {
            PrimaryButtonText = primaryMessage;
            SecondaryButtonText = secondaryMessage;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
