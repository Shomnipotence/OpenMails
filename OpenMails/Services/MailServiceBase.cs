using System.Collections.Generic;
using System.Threading;
using OpenMails.Models;

namespace OpenMails.Services
{
    public abstract class MailServiceBase : IMailService
    {
        public abstract string ServiceName { get; }
        public abstract string Name { get; }
        public abstract string Address { get; }

        public abstract IAsyncEnumerable<MailFolder> GetAllFoldersAsync(CancellationToken cancellationToken = default);
        public abstract IAsyncEnumerable<MailMessage> GetAllMessagesAsync(CancellationToken cancellationToken = default);
    }
}
