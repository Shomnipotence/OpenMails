using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.NativeInterop;

#nullable enable

namespace OpenMails.Services
{
    public class MailLoginService
    {
        static readonly string s_outlookLoginInstance = "https://login.microsoftonline.com/";
        static readonly string[] s_outlookLoginScopes = new string[] { "Mail.ReadWrite", "offline_access" };

        IPublicClientApplication _outlookLoginClient;

        /// <summary>
        /// 获取已登录的 Outlook 邮件服务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<IMailService?> GetLoginedOutlookServiceAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                IAccount firstAccount = Microsoft.Identity.Client.PublicClientApplication.OperatingSystemAccount;
                var authResult = await _outlookLoginClient.AcquireTokenSilent(s_outlookLoginScopes, firstAccount)
                    .ExecuteAsync(cancellationToken);

                return new OutlookMailService(authResult.AccessToken);
            }
            catch
            {
                return null;
            }
        }


        public MailLoginService()
        {
            _outlookLoginClient = PublicClientApplicationBuilder.Create(AppSecrets.MicrosoftGraphClientId)
                .WithClientName("OpenMails")
                .WithClientVersion(Assembly.GetExecutingAssembly().GetName().Version.ToString())
                .WithAuthority($"{s_outlookLoginInstance}{AppSecrets.MicrosoftGraphTenantId}")
                .WithDefaultRedirectUri()
                .WithBroker(true)
                .Build();
        }

        /// <summary>
        /// 获取所有已登录的邮件服务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<IMailService> GetLoginedMailServices(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (await GetLoginedOutlookServiceAsync(cancellationToken) is IMailService outlookService)
                yield return outlookService;
        }

        /// <summary>
        /// 登陆 Outlook 并取得其邮件服务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IMailService?> LoginOutlookAsync(
            CancellationToken cancellationToken = default)
        {
            AuthenticationResult? authResult = null;

            try
            {
                authResult = await _outlookLoginClient.AcquireTokenInteractive(s_outlookLoginScopes)
                    .WithPrompt(Microsoft.Identity.Client.Prompt.SelectAccount)
                    .ExecuteAsync();

                return new OutlookMailService(authResult.AccessToken);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
