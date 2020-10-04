using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FancyToys.Pages.Nursery
{

    public class FileUtil
    {
        private static string jsonFile = "Cache\\config.json";
        private static StorageFolder localFolder = ApplicationData.Current.LocalFolder;


        private struct FileProcessJson
        {
            public string pathname;
            public string args;
            public bool sourcefolder;  // 在源目录执行
        }



        public static async Task<Dictionary<string, NurseryPage.FileProcessStruct>> FileReader(Logger logger)
        {
            StorageFile file = await localFolder.CreateFileAsync(jsonFile, CreationCollisionOption.OpenIfExists);
            string JSONContent = await FileIO.ReadTextAsync(file);
            Debug.WriteLine(localFolder.Path);
            Dictionary<string, FileProcessJson> dj = JsonConvert.DeserializeObject<Dictionary<string, FileProcessJson>>(JSONContent);
            Dictionary<string, NurseryPage.FileProcessStruct> dict = new Dictionary<string, NurseryPage.FileProcessStruct>();
            if (dj != null)
            {
                foreach (KeyValuePair<string, FileProcessJson> kv in dj)
                {
                    dict.Add(kv.Key, new NurseryPage.FileProcessStruct()
                    {
                        pathname = kv.Value.pathname,
                        args = kv.Value.args,
                        sourcefolder = kv.Value.sourcefolder
                    });
                }
                Debug.WriteLine("已逆序列化文件");
            }
            Debug.WriteLine("dict: {0}", dict.Count);
            return dict;
        }

        public static async void FileWriter(Dictionary<string, NurseryPage.FileProcessStruct> dict, Logger logger)
        {
            Dictionary<string, FileProcessJson> dj = new Dictionary<string, FileProcessJson>();
            foreach (KeyValuePair<string, NurseryPage.FileProcessStruct> kv in dict)
            {
                dj.Add(kv.Key, new FileProcessJson()
                {
                    pathname = kv.Value.pathname,
                    args = kv.Value.args,
                    sourcefolder = kv.Value.sourcefolder
                });
            }

            string configJson = JsonConvert.SerializeObject(dj);
            StorageFile file = await localFolder.CreateFileAsync(jsonFile, CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(file, configJson);
            Debug.WriteLine("已写入文件");
        }
    }

    public class DialogUtil
    {
        public static ContentDialog ActiveDialog;
        static TaskCompletionSource<bool> DialogAwaiter = new TaskCompletionSource<bool>();
        public static async void CreateContentDialog(ContentDialog Dialog, bool awaitPreviousDialog) { await CreateDialog(Dialog, awaitPreviousDialog); }
        public static async Task CreateContentDialogAsync(ContentDialog Dialog, bool awaitPreviousDialog) { await CreateDialog(Dialog, awaitPreviousDialog); }
        private static void ActiveDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args) { DialogAwaiter.SetResult(true); }

        public static async void Info(string text)
        {
            await DialogUtil.CreateContentDialogAsync(new ContentDialog
            {
                Title = "Info",
                Content = new TextBlock
                {
                    Text = text,
                    TextWrapping = TextWrapping.Wrap
                },
                PrimaryButtonText = "好的"
            }, awaitPreviousDialog: true);
        }
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
            ActiveDialog = Dialog;
            ActiveDialog.Closed += ActiveDialog_Closed;
            await ActiveDialog.ShowAsync();
            ActiveDialog.Closed -= ActiveDialog_Closed;
        }
    }

}
