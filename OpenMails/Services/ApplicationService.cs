using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenMails.Services.Outlook;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.ApplicationModel.Core;
using OpenMails.ViewModels;

namespace OpenMails.Services
{
    /// <summary>
    /// 负责程序声明周期的服务
    /// </summary>
    internal class ApplicationService : IHostedService
    {
        private readonly NavigationService _navigationService;
        private readonly StorageService _storageService;
        private readonly AuthService _authService;

        public ApplicationService(
            NavigationService navigationService,
            StorageService storageService,
            AuthService authService)
        {
            _navigationService = navigationService;
            _storageService = storageService;
            _authService = authService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // 标题栏透明
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            // 初始化所有邮箱验证服务
            _authService.MailAuthServices.Add(new OutlookAuthService());

            // 获取所有已经登陆的邮箱服务
            List<IMailService> allLoginedServices = new();
            await foreach (var mailService in _authService.GetAllLoginedServicesAsync(cancellationToken))
                allLoginedServices.Add(mailService);

            // 将已经登陆的存入 MainPageViewModel
            var mainPageViewModel = App.Host.Services.GetRequiredService<MainPageViewModel>();
            foreach (var mailService in allLoginedServices)
                mainPageViewModel.MailServices.Add(mailService);

            // 根据登陆状态导航到对应页面
            if (allLoginedServices.Count == 0)
                _navigationService.NavigateToLoginPage();
            else
                _navigationService.NavigateToMainPage();

            // 激活窗口
            Window.Current.Activate();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
