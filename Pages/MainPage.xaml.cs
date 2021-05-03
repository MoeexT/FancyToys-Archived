using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Core.Preview;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;

using FancyToys.Pages;
using FancyToys.Pages.Dialog;
using FancyToys.Pages.Media;
using FancyToys.Pages.Nursery;
using FancyToys.Pages.Server;
using FancyToys.Pages.Settings;
using FancyToys.Bridge;
using Windows.UI.Core;
using System.Diagnostics;
using FancyToys.Pages.Teleport;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板
namespace FancyToys.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>()
        {
            ("nursery", typeof(NurseryPage)),
            ("trimage", typeof(TrimagePage)),
            ("teleport", typeof(TeleportPage)),
            //("avideo", typeof(AVideoPage)),
            ("fancyServer", typeof(ServerPage)),
        };

        public MainPage()
        {
            InitializeComponent();
            ContentFrame.CacheSize = 64;
            PipeBridge.Bridge.PipeOpened += OnServerConnected;
            PipeBridge.Bridge.PipeClosed += OnServerDisconnected;
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigated += On_Navigated;
            NavView.SelectedItem = NavView.MenuItems[0];
            NavView_Navigate("nursery", new EntranceNavigationTransitionInfo());

        }

        private void NavView_ItemInvoked(muxc.NavigationView sender, muxc.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                NavView_Navigate("settings", new DrillInNavigationTransitionInfo());
            }
            else if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, new DrillInNavigationTransitionInfo());
            }
        }

        private void NavView_SelectionChanged(muxc.NavigationView sender,
                                      muxc.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected == true)
            {
                NavView_Navigate("settings", new DrillInNavigationTransitionInfo());
            }
            else if (args.SelectedItemContainer != null)
            {
                var navItemTag = args.SelectedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, new DrillInNavigationTransitionInfo());
            }
        }

        private void NavView_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
            if (navItemTag == "settings")
            {
                _page = typeof(SettingsPage);
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;
            }
            var preNavPageType = ContentFrame.CurrentSourcePageType;

            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                ContentFrame.Navigate(_page, null, transitionInfo);
            }
        }


        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            //NavView.IsBackEnabled = ContentFrame.CanGoBack;
            if (ContentFrame.SourcePageType == typeof(SettingsPage))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
                NavView.SelectedItem = (muxc.NavigationViewItem)NavView.SettingsItem;
            }
            else if (ContentFrame.SourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);

                NavView.SelectedItem = NavView.MenuItems
                    .OfType<muxc.NavigationViewItem>()
                    .First(n => n.Tag.Equals(item.Tag));
            }
        }

        private void OnServerConnected(object sender, PipeBridge.PipeOpenedEventArgs e)
        {
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                FancyServer.Icon.Foreground = new SolidColorBrush(Colors.LightGreen);
            });
            
        }
        
        private void OnServerDisconnected(object sender, PipeBridge.PipeClosedEventArgs e)
        {
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                FancyServer.Icon.Foreground = new SolidColorBrush(Colors.OrangeRed);
            });
        }
    }
}
