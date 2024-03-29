using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenMails.Models;
using OpenMails.Services;

namespace OpenMails.ViewModels
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