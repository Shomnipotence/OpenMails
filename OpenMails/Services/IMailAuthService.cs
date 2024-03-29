using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OpenMails.Services
{
    public interface IMailAuthService
    {
        public string Name { get; }

        public IAsyncEnumerable<IMailService> GetLoginedServicesAsync(CancellationToken cancellationToken = default);
        public Task<IMailService?> LoginAsync(CancellationToken cancellationToken = default);
    }
}
