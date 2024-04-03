using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using MailApp.Abstraction;
using MailApp.Models;
using MailApp.Services;
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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MailApp.Views.Dialogs
{
    public sealed partial class LoginDialog : ContentDialog, ILoginHandler
    {
        public LoginDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

        public IEnumerable<MailAuthServiceWrapper> MailAuthServices
        {
            get
            {
                var authService = App.Host.Services.GetRequiredService<AuthService>();

                foreach (var mailAuthService in authService.MailAuthServices)
                    yield return new MailAuthServiceWrapper(mailAuthService);
            }
        }

        public void OnLoginStarted()
        {
            this.Hide();
        }

        public void OnLoginCompleted(IMailService mailService)
        {

        }
    }
}
