using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OpenMails.Services;
using OpenMails.ViewModels;
using OpenMails.Views;

namespace OpenMails.Models
{
    public partial class MailAuthServiceWrapper
    {
        public MailAuthServiceWrapper(
            IMailAuthService mailAuthService)
        {
            MailAuthService = mailAuthService;
        }

        public IMailAuthService MailAuthService { get; }

        public string Name => MailAuthService.Name;

        [RelayCommand]
        public async Task Login(CancellationToken cancellationToken)
        {
            var loginPage = App.Host.Services.GetRequiredService<LoginPage>();
            var mainPageViewModel = App.Host.Services.GetRequiredService<MainPageViewModel>();
            var mailService = await MailAuthService.LoginAsync(cancellationToken);

            if (mailService is not null)
            {
                // 仅在不存在相同服务时添加
                if (!mainPageViewModel.MailServices
                    .Any(service => service.Name == mailService.Name 
                        && service.Address == mailService.Address))
                {
                    mainPageViewModel.MailServices.Add(mailService);
                }

                loginPage.OnLoginCompleted();
            }
        }
    }
}
