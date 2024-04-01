using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using MailApp.Models;
using MailApp.Services;
using MailApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

#nullable enable

namespace MailApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MailsPage : Page
    {
        private WebView2? _webView2;

        public MailsPage()
        {
            DataContext = this;
            ViewModel = App.Host.Services.GetRequiredService<MailsPageViewModel>();

            this.InitializeComponent();
        }

        public MailsPageViewModel ViewModel { get; }

        public void UpdateContent(IMailService? mailService, MailFolder? mailFolder)
        {
            ViewModel.MailService = mailService;
            ViewModel.Folder = mailFolder;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is not MailServiceAndFolder mailServiceAndFolder)
                return;

            UpdateContent(mailServiceAndFolder.Service, mailServiceAndFolder.Folder);
        }

        private void Page_Loading(FrameworkElement sender, object args)
        {
            ViewModel.ClearCache();
        }

        /// <summary>
        /// WebView2 加载完毕时, 进行初始化操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebView2_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not WebView2 webview2)
                return;

            // webview2 initialization

            // set webview2 background as transparent
            Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", "00FFFFFF");

            // create core webview2
            _ = webview2.EnsureCoreWebView2Async();

            // save webveiw2 instance
            _webView2 = webview2;
        }

        /// <summary>
        /// WebView2 CoreWebView2 初始化完毕时加载当前选中邮件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void WebView2_CoreWebView2Initialized(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.UI.Xaml.Controls.CoreWebView2InitializedEventArgs args)
        {
            if (sender.Tag is not MailMessage mailMessage)
                return;

            sender.CoreWebView2.NavigateToString(mailMessage.Content.Text);
        }

        /// <summary>
        /// 当 WebView2 开始导航时, 如果是 HTTP/HTTPS 链接则通过系统默认应用打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void WebView2_NavigationStarting(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            if (Uri.TryCreate(args.Uri, UriKind.RelativeOrAbsolute, out var uri))
            {
                if (uri.Scheme.Equals("HTTP", StringComparison.OrdinalIgnoreCase) ||
                    uri.Scheme.Equals("HTTPS", StringComparison.OrdinalIgnoreCase))
                {
                    args.Cancel = true;
                    await global::Windows.System.Launcher.LaunchUriAsync(uri);
                }
            }
        }

        /// <summary>
        /// 当选择的邮件改变时, 使 WebView2 导航
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MailMessageListDetailsView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_webView2 is null || _webView2.CoreWebView2 is null)
                return;
            if (sender is not ListDetailsView listDetailsView ||
                listDetailsView.SelectedItem is not MailMessage selectedMessage)
                return;

            _webView2.CoreWebView2.NavigateToString(selectedMessage.Content.Text);
        }
    }
}
