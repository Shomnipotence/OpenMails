using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace MailApp.Models
{
    public record class MailMessage
    {
        [BsonCtor]
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

        public Recipient Sender { get; set; }
        public ICollection<Recipient> To { get; set; }
        public ICollection<Recipient> Cc { get; set; }
        public string Overview { get; set; }
        public MailMessageContent Content { get; set; }

        /// <summary>
        /// 此消息的 ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 包含此消息的文件夹的 ID
        /// </summary>
        public string ContainingFolderId { get; set; }
    }
}
