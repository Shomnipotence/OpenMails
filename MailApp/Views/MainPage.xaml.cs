using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.UI.Xaml.Controls;
using MailApp.Models;
using MailApp.Services;
using MailApp.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.VisualBasic;

#nullable enable

namespace MailApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            DataContext = this;
            ViewModel = App.Host.Services.GetRequiredService<MainPageViewModel>();
            GlobalData = App.Host.Services.GetRequiredService<ApplicationGlobalData>();
            Strings = App.Host.Services.GetRequiredService<I18nStrings>();

            this.InitializeComponent();

            if (ViewModel.SelectedMailService is null)
                ViewModel.SelectedMailService = GlobalData.MailServices.FirstOrDefault();

            Window.Current.SetTitleBar(titleBarDragingArea);
        }

        public MainPageViewModel ViewModel { get; }
        public ApplicationGlobalData GlobalData { get; }
        public I18nStrings Strings { get; }

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (sender.SelectedItem is not TreeNode<MailFolder> folderNode)
                return;

            if (navigationFrame.Content is MailsPage currentMailsPage)
            {
                currentMailsPage.UpdateContent(ViewModel.SelectedMailService, ViewModel.CurrentAllMailFolders, folderNode.Value);
            }
            else
            {
                navigationFrame.Navigate(
                    typeof(MailsPage),
                    new MailServiceStatus(ViewModel.SelectedMailService, ViewModel.CurrentAllMailFolders, folderNode.Value));
            }
        }

        private void NavigationView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                navigationFrame.Navigate(
                    typeof(SettingsPage));
            }
        }
    }
}
