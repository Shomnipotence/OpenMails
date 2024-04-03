using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using MailApp.Services;
using MailApp.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MailApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        readonly NavigationService _navigationService = App.Host.Services.GetRequiredService<NavigationService>();

        public SettingsPage()
        {
            DataContext = this;
            ViewModel = App.Host.Services.GetRequiredService<SettingsPageViewModel>();
            GlobalData = App.Host.Services.GetRequiredService<ApplicationGlobalData>();
            Strings = App.Host.Services.GetRequiredService<I18nStrings>();

            this.InitializeComponent();
        }

        public SettingsPageViewModel ViewModel { get; }
        public ApplicationGlobalData GlobalData { get; }
        public I18nStrings Strings { get; }

        [RelayCommand]
        public async Task AddMail()
        {
            var loginDialog = new LoginDialog();
            await loginDialog.ShowAsync();
        }

        [RelayCommand]
        public async Task LogoutMail(IMailService mailService)
        {
            await mailService.AuthService.LogoutAsync(mailService);
            GlobalData.MailServices.Remove(mailService);

            if (GlobalData.MailServices.Count == 0)
            {
                _navigationService.NavigateToLoginPage();
            }
        }
    }
}
