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
    public enum CloseAction
    {
        Terminate,
        Systray,
        Consolidate
    }
    public sealed partial class ConfirmCloseDialog : ContentDialog
    {
        public CloseAction Result { get; private set; }

        public ConfirmCloseDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (rbTerminate.IsChecked == true)
            {
                Result = CloseAction.Terminate;
            }
            else if(rbSystray.IsChecked == true)
            {
                Result = CloseAction.Systray;
            } 
            else
            {
                Result = CloseAction.Consolidate;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
