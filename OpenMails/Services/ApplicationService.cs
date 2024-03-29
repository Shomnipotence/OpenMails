using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Windows.UI.Xaml;

namespace OpenMails.Services
{
    internal class ApplicationService : IHostedService
    {
        private readonly NavigationService _navigationService;
        private readonly AuthService _authService;

        public ApplicationService(
            NavigationService navigationService,
            AuthService authService)
        {
            _navigationService = navigationService;
            _authService = authService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _authService.MailAuthServices.Add(new OutlookAuthService());

            List<IMailService> allLoginedServices = new();
            await foreach (var mailService in _authService.GetAllLoginedServicesAsync(cancellationToken))
                allLoginedServices.Add(mailService);

            if (allLoginedServices.Count == 0)
                _navigationService.NavigateToLoginPage();
            else
                _navigationService.NavigateToMainPage();

            Window.Current.Activate();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
