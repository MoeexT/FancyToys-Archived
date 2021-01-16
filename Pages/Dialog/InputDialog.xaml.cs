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
    public sealed partial class InputDialog : ContentDialog
    {
        public bool isSaved = false;
        public string inputArgs = "";

        public InputDialog()
        {
            this.InitializeComponent();
        }
        public InputDialog(string text)
        {
            this.InitializeComponent();
            DialogText.Text = text;
        }
        public InputDialog(string text, string dargs)
        {
            this.InitializeComponent();
            DialogText.Text = text;
            DialogInput.Text = dargs;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            inputArgs = DialogInput.Text;
            isSaved = true;
            Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            isSaved = false;
            Hide();
        }
    }
}
