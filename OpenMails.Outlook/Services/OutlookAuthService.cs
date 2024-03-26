using System.Reflection;
using Microsoft.Identity.Client;

namespace OpenMails.Services
{
    public class OutlookAuthService
    {
        static readonly string s_outlookLoginInstance = "https://login.microsoftonline.com/";
        static readonly string[] s_outlookLoginScopes = new string[] { "Mail.ReadWrite", "offline_access" };

        IPublicClientApplication _outlookLoginClient;

        /// <summary>
        /// 获取已登录的 Outlook 邮件服务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IMailService?> GetLoginedServiceAsync(
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


        public OutlookAuthService()
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
