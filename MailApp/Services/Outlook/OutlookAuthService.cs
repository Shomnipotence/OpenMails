using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

#nullable enable

namespace MailApp.Services.Outlook
{
    public class OutlookAuthService : IMailAuthService
    {
        static readonly string s_cacheFileName = "AuthCache";
        static readonly string s_outlookLoginInstance = "https://login.microsoftonline.com/";
        static readonly string[] s_outlookLoginScopes = ["User.Read", "Mail.ReadWrite", "offline_access"];

        IPublicClientApplication _identityClient;

        public string Name => "Outlook";

        /// <summary>
        /// 获取已登录的 Outlook 邮件服务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<IMailService> GetLoginedServicesAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var accounts = await _identityClient.GetAccountsAsync();

            foreach (var account in accounts)
            {
                IMailService? currentMailService = null;

                try
                {
                    var authResult = await _identityClient.AcquireTokenSilent(s_outlookLoginScopes, account)
                    .ExecuteAsync(cancellationToken);

                    currentMailService = new OutlookMailService(this, authResult.Account, authResult.AccessToken);
                }
                catch { }

                if (currentMailService is not null)
                    yield return currentMailService;
            }
        }


        public OutlookAuthService()
        {
            _identityClient = PublicClientApplicationBuilder.Create(AppSecrets.MicrosoftGraphClientId)
                .WithClientName("MailApp")
                .WithClientVersion(Assembly.GetExecutingAssembly().GetName().Version.ToString())
                .WithAuthority($"{s_outlookLoginInstance}{AppSecrets.MicrosoftGraphTenantId}")
                .WithUseCorporateNetwork(false)
                .WithDefaultRedirectUri()
                .WithBroker(true)
                .Build();
        }


        /// <summary>
        /// 登陆 Outlook 并取得其邮件服务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IMailService?> LoginAsync(
            CancellationToken cancellationToken = default)
        {
            AuthenticationResult? authResult = null;

            try
            {
                authResult = await _identityClient.AcquireTokenInteractive(s_outlookLoginScopes)
                    .WithPrompt(Microsoft.Identity.Client.Prompt.SelectAccount)
                    .ExecuteAsync();

                return new OutlookMailService(this, authResult.Account, authResult.AccessToken);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Task LogoutAsync(IMailService mailService, CancellationToken cancellationToken = default)
        {
            if (mailService is not OutlookMailService outlookMailService)
            {
                throw new ArgumentException("Specified service is not outlook mail service", nameof(mailService));
            }

            _identityClient.RemoveAsync(outlookMailService.Account);

            return Task.CompletedTask;
        }
    }
}
