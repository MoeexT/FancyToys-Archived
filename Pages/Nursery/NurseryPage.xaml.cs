using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using System.Diagnostics;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.Storage;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Controls.Primitives;

using FancyToys.Pages.Dialog;
using Microsoft.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FancyToys.Pages.Nursery
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NurseryPage : Page
    {
        Dictionary<string, FileProcessStruct> pNersury; // = new Dictionary<string, FileProcessStruct>();
        //private HashSet<string> selectedSet = new HashSet<string>();
        private string lastFile;

        public NurseryPage()
        {
            this.InitializeComponent();
            Initialize();
            InitializeLauncher();
        }

        //private void Rectangle_Drop(object sender, DragEventArgs e)
        //{
        //    ContentDialog contentDialog = new ContentDialog()
        //    {
        //        Title = "Drop Test",
        //        PrimaryButtonText = "Save",
        //        SecondaryButtonText = "Don't Save",
        //        CloseButtonText = "Cancel",
        //        DefaultButton = ContentDialogButton.Primary
        //    };
        //    _ = contentDialog.ShowAsync();
        //}
        //private void Rectangle_DragEnter(object sender, DragEventArgs e)
        //{
        //    ImageBrush brush = new ImageBrush();
        //    brush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/sunny.jpg", UriKind.Absolute));
        //    DropArea.Background = brush;
        //}
        //private void Rectangle_DragOver(object sender, DragEventArgs e)
        //{
        //    DropArea.Background = null;
        //}

        




        /// <summary>
        /// 【已弃用】
        /// 点击关闭之后要做的事情
        /// 1. 取消关闭
        /// 2. 隐藏窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    e.Cancel = true;
        //    //this.Hide();
        //    Debug.WriteLine("window hided");
        //}


        /// 【恢复】
        //private void ExitApp(object sender, RoutedEventArgs e)
        //{
        //    Utils.FileWriter(pNersury, logger);
        //    StopAllProcesses();
        //    Debug.WriteLine("程序即将退出");
        //    Application.Current.Shutdown();
        //}

        //选中checkbox之后要处理的事：
        //1. 把内容（文件名/进程名）添加到`selectedSet`
        //2. 更新最近选择的checkBox：`recentCheckedFile`
        //3. 把该进程执行前需要的参数显示到TextBox上
        //private void CheckBox_Checked(object sender, RoutedEventArgs e)
        //{
        //    CheckBox checkBox = (CheckBox)sender;
        //    if (!selectedSet.Contains(checkBox.Content))
        //    {
        //        Debug.WriteLine("CheckBox checked: " + checkBox.Content);
        //        selectedSet.Add((string)checkBox.Content);
        //        recentCheckedFile = (string)checkBox.Content;
        //        ArgsTextBox.Text = pNersury[recentCheckedFile].args;
        //        ArgsTextBox.Focus(FocusState.Programmatic);
        //        ArgsTextBox.SelectAll();
        //    }
        //}


        //取消选择checkbox要做的事情：
        //1. 在`selectedSet`中移除该checkbox
        //2. 清空textbox中的内容
        //private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    CheckBox checkBox = (CheckBox)sender;
        //    if (selectedSet.Contains(checkBox.Content))
        //    {
        //        selectedSet.Remove((string)checkBox.Content);
        //        ArgsTextBox.Text = "";
        //    }
        //}


        /// <summary>
        /// 拖拽添加文件，过滤掉非exe的文件。把这些文件添加到他们该去的地方
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DropArea_Drop(object sender, DragEventArgs e)
        {
            var defer = e.GetDeferral();
            try
            {
                DataPackageView dpv = e.DataView;
                if (dpv.Contains(StandardDataFormats.StorageItems))
                {
                    //List<StorageFile> fileList = new List<StorageFile>();
                    var files = await dpv.GetStorageItemsAsync();
                    foreach (var item in files)
                    {
                        if (item.Name.EndsWith(".exe"))
                        {
                            AddFile(item.Path);
                        }
                    }
                }
            }
            finally
            {
                defer.Complete();
            }
        }
        private void DropArea_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "拖放以添加";
            e.DragUIOverride.IsCaptionVisible = true;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
            e.Handled = true;
        }




        /// <summary>
        /// 移除按钮
        /// 1. 在listbox中移除已选择的checkbox
        /// 2. 在`pNursery`中移除相应的进程信息
        /// 3. 清空`selectedSet`
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void RemoveButton_Click(object sender, RoutedEventArgs e)
        //{
        //    foreach (string f in selectedSet)
        //    {
        //        ProcessListBox.Items.Remove(pNersury[f].checkbox);
        //        pNersury.Remove(f);
        //        Debug.WriteLine("Removed: " + f);
        //    }
        //    selectedSet.Clear();
        //    logger.Debug("selectedSet cleared");
        //}

        private void Switch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch tswitch = sender as ToggleSwitch;
            Debug.Assert((ToggleSwitch)sender != null);
            if (tswitch != null)
            {
                string file = tswitch.OnContent.ToString().Split(' ')[0];
                FileProcessStruct ps = pNersury[file];

                if (tswitch.IsOn == true)
                {
                    if (!String.IsNullOrEmpty(file))
                    {
                        SendMessage(true, ps.pathname, ps.args);
                        ps.isRunning = true;
                        pNersury[file] = ps;
                        AddProcessInformation(file);
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(file))
                    {
                        if (ps.isRunning)
                        {
                            SendMessage(false, ps.pathname, ps.args);
                            ps.isRunning = false;
                        }
                        Debug.WriteLine("已终止: " + file);
                        pNersury[file] = ps;
                        RemoveProcessInformation(file);
                    }
                    
                }
                return;
            }
        }

        private void Switch_RightTapped(object sender, RoutedEventArgs e)
        {
            ToggleSwitch tswitch = sender as ToggleSwitch;
            lastFile = tswitch.OnContent.ToString().Split(' ')[0];
        }

        /// <summary>
        /// 手动添加文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.HomeGroup;
            picker.FileTypeFilter.Add(".exe");
            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                AddFile(file.Path);
            }
            FileUtil.FileWriter(pNersury);
        }
        private void StopAllFlyoutItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveAllFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
        }
        private void HelpFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
        }

        private void AboutFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// 开关的右键唤出菜单  附加参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AppBarArgsButton_Click(object sender, RoutedEventArgs e)
        {
            InputDialog inputDialog = new InputDialog("为"+ lastFile +"输入参数", pNersury[lastFile].args);
            await inputDialog.ShowAsync();
            if (inputDialog.isSaved)
            {
                ModifyNursery(lastFile, inputDialog.inputArgs);
                FileUtil.FileWriter(pNersury);
            }
        }

        /// <summary>
        /// 开关的右键删除按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AppBarDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //DialogUtil.Info("我要深处啦");
            ToggleSwitch rts = null;
            foreach (ToggleSwitch ts in ProcessListBox.Items)
            {
                if (ts.OnContent.ToString().StartsWith(lastFile))
                {
                    if (ts.IsOn)
                    {
                        //await MessageDialog.Warn("进程未退出", "继续操作可能丢失工作内容");
                        MessageDialog dialog = new MessageDialog("进程未退出", "继续操作可能丢失工作内容", MessageDialog.MessageType.Info)
                        {
                            PrimaryButtonText = "仍然退出"
                        };
                        dialog.PrimaryButtonClick += (_s, _e) => 
                        {
                            FileProcessStruct ps = pNersury[lastFile];
                            ps.isRunning = false;
                            SendMessage(false, ps.pathname, ps.args);
                            pNersury[lastFile] = ps;
                            RemoveProcessInformation(lastFile);
                            rts = ts;
                        };
                        await dialog.ShowAsync();
                    } else {
                        rts = ts;
                        break;
                    }
                }
            }
            if (rts != null)
            {
                ProcessListBox.Items.Remove(rts);
                pNersury.Remove(lastFile);
                FileUtil.FileWriter(pNersury);
            }
        }

        private void SetSizeFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ProcessListBox.ItemContainerStyle = SetStyle(HeightProperty, ((RadioMenuFlyoutItem)sender).Tag);
        }
        /// <summary>
        /// 动态改变switchToggle的样式
        /// From https://blog.csdn.net/lindexi_gd/article/details/104992276
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private Style SetStyle(DependencyProperty property, object value)
        {
            Style style = new Style
            {
                TargetType = typeof(ListBoxItem)
            };
            style.Setters.Add(new Setter(property, value));
            style.Setters.Add(new Setter(PaddingProperty, "10,0,0,0"));
            ProcessListBox.ItemContainerStyle = style;
            return style;
        }

    }
}
