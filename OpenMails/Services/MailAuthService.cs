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
    public class MailAuthService
    {
        private readonly OutlookAuthService _outlookAuthService;

        public MailAuthService(
            OutlookAuthService outlookAuthService)
        {
            _outlookAuthService = outlookAuthService;
        }

        /// <summary>
        /// 获取所有已登录的邮件服务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<IMailService> GetLoginedMailServices(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (await  _outlookAuthService.GetLoginedServiceAsync(cancellationToken) is IMailService outlookService)
                yield return outlookService;
        }

        public Task<IMailService?> LoginOutlookAsync()
            => _outlookAuthService.LoginAsync();
    }
}
