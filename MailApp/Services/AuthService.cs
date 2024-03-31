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

namespace MailApp.Services
{
    /// <summary>
    /// 验证服务 <br/>
    /// 可以理解为所有邮箱验证服务的容器
    /// </summary>
    public class AuthService
    {
        public AuthService()
        {

        }

        public List<IMailAuthService> MailAuthServices { get; } = new();

        public async IAsyncEnumerable<IMailService> GetAllLoginedServicesAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            foreach (var mailAuthService in MailAuthServices)
            {
                await foreach (var mailService in mailAuthService.GetLoginedServicesAsync())
                    yield return mailService;
            }
        }
    }
}
