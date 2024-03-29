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
