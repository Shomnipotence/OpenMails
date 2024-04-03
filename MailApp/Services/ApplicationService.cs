using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MailApp.Services.Outlook;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.ApplicationModel.Core;
using MailApp.ViewModels;
using MailApp.Services.Gmail;

namespace MailApp.Services
{

    /// <summary>
    /// 负责程序声明周期的服务
    /// </summary>
    public class ApplicationService : IHostedService
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
            // 标题栏透明
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            // 初始化语言
            var i18nStrings = App.Host.Services.GetRequiredService<I18nStrings>();
            i18nStrings.AllowFallback = true;
            i18nStrings.AllowFuzzyMatching = true;
            i18nStrings.CurrentCulture = i18nStrings.AllCultures.FirstOrDefault();

            // 初始化所有邮箱验证服务
            _authService.MailAuthServices.Add(new OutlookAuthService());
            _authService.MailAuthServices.Add(new GMailAuthService());

            // 获取所有已经登陆的邮箱服务
            List<IMailService> allLoginedServices = new();
            await foreach (var mailService in _authService.GetAllLoginedServicesAsync(cancellationToken))
                allLoginedServices.Add(mailService);

            // 将已经登陆的存入全局数据
            var globalData = App.Host.Services.GetRequiredService<ApplicationGlobalData>();
            foreach (var mailService in allLoginedServices)
                globalData.MailServices.Add(mailService);

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
            var cacheService = App.Host.Services
                .GetRequiredService<CacheService>();

            cacheService.CloseAll();

            return Task.CompletedTask;
        }
    }
}
