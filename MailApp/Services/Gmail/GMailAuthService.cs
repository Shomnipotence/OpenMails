using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace MailApp.Services.Gmail
{
    public class GMailAuthService : IMailAuthService
    {
        public string Name => "Gmail";

        public async IAsyncEnumerable<IMailService> GetLoginedServicesAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            yield break;
        }

        public async Task<IMailService?> LoginAsync(CancellationToken cancellationToken = default)
        {
            ContentDialog contentDialog = new()
            {
                Title = "Tip",
                Content = "Not implemented",
                CloseButtonText = "Ok"
            };

            await contentDialog.ShowAsync();

            return null;
        }

        public Task LogoutAsync(IMailService mailService, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
