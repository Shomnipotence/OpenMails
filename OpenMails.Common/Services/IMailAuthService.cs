using System.Runtime.CompilerServices;

namespace OpenMails.Services
{
    public interface IMailAuthService
    {
        public string Name { get; }

        public IAsyncEnumerable<IMailService> GetLoginedServicesAsync(CancellationToken cancellationToken = default);
        public Task<IMailService?> LoginAsync(CancellationToken cancellationToken = default);
    }
}
