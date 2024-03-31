using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MailApp.Models;
using MailApp.Services;

namespace MailApp.ViewModels
{
    public partial class LoginPageViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        public LoginPageViewModel(
            AuthService authService)
        {
            _authService = authService;
        }

        public IEnumerable<MailAuthServiceWrapper> AllMailAuthServices
        {
            get
            {
                foreach (var mailAuthService in _authService.MailAuthServices)
                    yield return new MailAuthServiceWrapper(mailAuthService);
            }
        }
    }
}