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
using System.Reflection;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FancyToys.Pages.Nursery
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NurseryPage : Page
    {
        private static NurseryPage page;

        public static NurseryPage Page { get => page; private set => page = value; }


        public NurseryPage()
        {
            this.InitializeComponent();
            Page = this;
            LoggingManager.Debug($"Created a NurseryPage instance", 2);
        }

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
                    var files = await dpv.GetStorageItemsAsync();
                    foreach (var item in files)
                    {
                        if (item.Name.EndsWith(".exe"))
                        {
                            TryAddFile(item.Path);
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

        private void Switch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch tswitch = sender as ToggleSwitch;
            if (tswitch != null)
            {
                string pathName = tswitch.Tag as string;

                if (tswitch.IsOn == true)
                {
                    OperationClerk.TryStart(pathName, fargs[pathName]);
                }
                else
                {
                    OperationClerk.TryStop(pathName);
                }
                return;
            }
        }

        /// <summary>
        /// 手动添加文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddFileFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.HomeGroup
            };
            picker.FileTypeFilter.Add(".exe");
            StorageFile file = await picker.PickSingleFileAsync();
            // TODO: 可能选择多个文件
            if (file != null)
            {
                TryAddFile(file.Path);
            }
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
        private async void ArgsButton_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem ai = sender as MenuFlyoutItem;
            InputDialog inputDialog = new InputDialog("Nursery", "输入参数", fargs[ai.Tag as string]);
            await inputDialog.ShowAsync();
            if (inputDialog.isSaved)
            {
                fargs[ai.Tag as string] = inputDialog.inputArgs;
            }
        }

        /// <summary>
        /// 开关的右键删除按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem ri = sender as MenuFlyoutItem;
            ToggleSwitch rts = null;
            bool confirm = true;
            foreach (ToggleSwitch ts in ProcessListBox.Items)
            {
                if (ts.Tag.Equals(ri.Tag))
                {
                    rts = ts;
                    if (ts.IsOn)
                    {
                        confirm &= await MessageDialog.Warn("进程未退出", "继续操作可能丢失工作内容", "仍然退出");
                    }
                }
            }
            if (rts != null && confirm)
            {
                OperationClerk.TryStop(ri.Tag as string);
                OperationClerk.TryRemove(ri.Tag as string);
            }
        }

        private void ListBoxSize_Click(object sender, RoutedEventArgs e)
        {
            ProcessListBox.ItemContainerStyle = SetStyle(HeightProperty, ((MenuFlyoutItem)sender).Tag);
        }

        private void DataGridSize_Click(object sender, RoutedEventArgs e)
        {
            
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
