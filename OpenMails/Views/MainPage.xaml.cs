using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.UI.Xaml.Controls;
using OpenMails.Models;
using OpenMails.Services;
using OpenMails.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OpenMails.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        WebView2 _webView2;

        public MainPage(
            MainPageViewModel viewModel)
        {
            DataContext = this;
            ViewModel = viewModel;

            this.InitializeComponent();
        }

        public MainPageViewModel ViewModel { get; }

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

        private void WebView2_CoreWebView2Initialized(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.UI.Xaml.Controls.CoreWebView2InitializedEventArgs args)
        {
            if (sender.Tag is not MailMessage mailMessage)
                return;

            sender.CoreWebView2.NavigateToString(mailMessage.Content.Text);
        }

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
