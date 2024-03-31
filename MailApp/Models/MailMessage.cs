using System;
using System.Collections.Generic;
using System.Linq;

namespace MailApp.Models
{
    public record class MailMessage
    {
        public MailMessage(
            string id,
            string subject,
            Recipient sender,
            IEnumerable<Recipient> to,
            IEnumerable<Recipient> cc,
            string overview,
            MailMessageContent content,
            string containingFolderId)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Sender = sender ?? throw new ArgumentNullException(nameof(sender));
            To = to?.ToArray() ?? throw new ArgumentNullException(nameof(to));
            Cc = cc?.ToArray() ?? throw new ArgumentNullException(nameof(cc));
            Overview = overview ?? throw new ArgumentNullException(nameof(overview));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            ContainingFolderId = containingFolderId ?? throw new ArgumentNullException(nameof(containingFolderId));
        }

        public Recipient Sender { get; }
        public ICollection<Recipient> To { get; }
        public ICollection<Recipient> Cc { get; }
        public string Overview { get; }
        public MailMessageContent Content { get; }

        public string Id { get; }
        public string Subject { get; }
        public string ContainingFolderId { get; }
    }
}
