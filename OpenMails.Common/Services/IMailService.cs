using System;
using System.Collections.Generic;
using System.Text;
using OpenMails.Models;

namespace OpenMails.Services
{
    public interface IMailService
    {
        public string Name { get; }
        public string Address { get; }

        public IAsyncEnumerable<MailFolder> GetAllFoldersAsync(CancellationToken cancellationToken = default);
        public IAsyncEnumerable<MailMessage> GetAllMessagesAsync(CancellationToken cancellationToken = default);
    }
}
