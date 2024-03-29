using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMails.Models
{
    public class MailMessage
    {
        public MailMessage(
            Recipient sender,
            IEnumerable<Recipient> to,
            IEnumerable<Recipient> cc,
            MailMessageContent content)
        {
            Sender = sender ?? throw new ArgumentNullException(nameof(sender));
            To = to?.ToArray() ?? throw new ArgumentNullException(nameof(to));
            Cc = cc?.ToArray() ?? throw new ArgumentNullException(nameof(cc));
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public Recipient Sender { get; }
        public ICollection<Recipient> To { get; }
        public ICollection<Recipient> Cc { get; }
        public MailMessageContent Content { get; }
    }
}
