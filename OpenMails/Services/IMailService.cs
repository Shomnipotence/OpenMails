using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenMails.Models;
using Windows.UI.Xaml.Media;

namespace OpenMails.Services
{
    public interface IMailService
    {
        public string ServiceName { get; }

        public string Name { get; }
        public string Address { get; }
        public ImageSource Avatar { get; }

        public IAsyncEnumerable<MailFolder> GetAllFoldersAsync(CancellationToken cancellationToken = default);
        public IAsyncEnumerable<MailMessage> GetAllMessagesAsync(CancellationToken cancellationToken = default);

        public IAsyncEnumerable<MailMessage> GetAllMessagesInFolder(MailFolder folder, CancellationToken cancellationToken = default);
        public IAsyncEnumerable<MailMessage> GetMessagesInFolder(MailFolder folder, int skip, int take, CancellationToken cancellationToken = default);
    }
}
