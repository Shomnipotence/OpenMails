using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MailApp.Services;
using MailApp.ViewModels;
using MailApp.Views;

namespace MailApp.Models
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
            var globalData = App.Host.Services.GetRequiredService<ApplicationGlobalData>();
            var newMailService = await MailAuthService.LoginAsync(cancellationToken);

            if (newMailService is not null)
            {
                // 仅在不存在相同服务时添加
                if (!globalData.MailServices.Any(mailService => mailService.Name == newMailService.Name && mailService.Address == newMailService.Address))
                {
                    globalData.MailServices.Add(newMailService);
                }

                loginPage.OnLoginCompleted();
            }
        }
    }
}
