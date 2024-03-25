using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using OpenMails.Services;
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

namespace OpenMails
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            _testTextBlock.Text = AppSecrets.MicrosoftGraphClientId;

            Loaded += MainPage_Loaded;


        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var service = await App.Services
                    .GetRequiredService<MailLoginService>()
                    .LoginOutlookAsync();

                StringBuilder sb = new();
                sb.AppendLine("Mail Messages:");
                await foreach (var message in service.GetAllMessagesAsync(default))
                {
                    sb.AppendLine($"{message.Sender.DisplayName}: {message.Content.Text}");
                }

                _testTextBlock.Text = sb.ToString();
            }
            catch
            {

            }
        }
    }
}
