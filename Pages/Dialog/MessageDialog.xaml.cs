using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace FancyToys.Pages.Dialog
{
    public sealed partial class MessageDialog : ContentDialog
    {
        public enum MessageType
        {
            Debug,
            Info,
            Warn,
            Error,
        }
        private static Dictionary<MessageType, string> IconDict = new Dictionary<MessageType, string>
        {
            { MessageType.Debug, ((char)0xEBE8).ToString()},
            { MessageType.Info, ((char)0xEA80).ToString()},
            { MessageType.Warn, ((char)0xE7BA).ToString()},
            { MessageType.Error, ((char)0xEDAE).ToString()},
        };
        private static Dictionary<MessageType, Brush> ColorDict = new Dictionary<MessageType, Brush>
        {
            { MessageType.Debug, new SolidColorBrush(Colors.PaleGreen)},
            { MessageType.Info, new SolidColorBrush(Colors.Cyan)},
            { MessageType.Warn, new SolidColorBrush(Colors.Yellow)},
            { MessageType.Error, new SolidColorBrush(Colors.Red)},
        };

        public MessageDialog()
        {
            this.InitializeComponent();
        }
        public MessageDialog(string title, string message, MessageType type)
        {
            this.InitializeComponent();

            TitleIcon.Glyph = IconDict[type];
            TitleIcon.Foreground = ColorDict[type];
            TitleText.Text = title;
            TheTextBlock.Text = message;
            TheTextBlock.TextWrapping = TextWrapping.WrapWholeWords;
            DefaultButton = ContentDialogButton.Secondary;
            TheImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/wj.jpg"));
        }


        // https://stackoverflow.com/questions/33018346/only-a-single-contentdialog-can-be-open-at-any-time-error-while-opening-anoth
        public static MessageDialog ActiveDialog;
        static TaskCompletionSource<bool> DialogAwaiter = new TaskCompletionSource<bool>();
        public static async void CreateMessageDialog(MessageDialog Dialog, bool awaitPreviousDialog) { await CreateDialog(Dialog, awaitPreviousDialog); }
        public static async Task CreateMessageDialogAsync(MessageDialog Dialog, bool awaitPreviousDialog) { await CreateDialog(Dialog, awaitPreviousDialog); }
        private static void ActiveDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args) { DialogAwaiter.SetResult(true); }
        static async Task CreateDialog(ContentDialog Dialog, bool awaitPreviousDialog)
        {
            if (ActiveDialog != null)
            {
                if (awaitPreviousDialog)
                {
                    await DialogAwaiter.Task;
                    DialogAwaiter = new TaskCompletionSource<bool>();
                }
                else ActiveDialog.Hide();
            }
            ActiveDialog = (MessageDialog)Dialog;
            ActiveDialog.Closed += ActiveDialog_Closed;
            await ActiveDialog.ShowAsync();
            ActiveDialog.Closed -= ActiveDialog_Closed;
        }

        public static async Task Debug(string title, string message)
        {
            await CreateMessageDialogAsync(new MessageDialog(title, message, MessageType.Debug)
            {
            }, awaitPreviousDialog: true);
        }
        //
        public static async Task Info(string title, string message)
        {
            await CreateMessageDialogAsync(new MessageDialog(title, message, MessageType.Info)
            {
            }, awaitPreviousDialog: true);
        }
        public static async Task Warn(string title, string message)
        {
            await CreateMessageDialogAsync(new MessageDialog(title, message, MessageType.Warn)
            {
            }, awaitPreviousDialog: true);
        }
        public static async Task Error(string title, string message)
        {
            await CreateMessageDialogAsync(new MessageDialog(title, message, MessageType.Error)
            {
            }, awaitPreviousDialog: true);
        }

        private void DragImageOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "拖放以更换";
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
            e.Handled = true;
        }

        private async void DropImageIn(object sender, DragEventArgs e)
        {
            var defer = e.GetDeferral();
            try
            {
                DataPackageView dpv = e.DataView;
                if (dpv.Contains(StandardDataFormats.StorageItems))
                {
                    var files = await dpv.GetStorageItemsAsync();
                    foreach (var item in files)
                    {
                        if (item.Name.EndsWith(".jpg") || item.Name.EndsWith(".jpeg") || item.Name.EndsWith(".png"))
                        {
                            StorageFile file = StorageFile.GetFileFromPathAsync(item.Path).GetResults();
                            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                            {
                                BitmapImage bitmapImage = new BitmapImage();
                                await bitmapImage.SetSourceAsync(fileStream);
                                TheImage.Source = bitmapImage;
                            }
                            break;
                        }
                    }
                }
            }
            finally
            {
                defer.Complete();
            }
        }
    }
}
