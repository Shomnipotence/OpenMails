using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenMails.Models;

namespace OpenMails.Extensions
{
    internal static class OutlookExtensions
    {
        public static MailMessage ToCommonMailMessage(this Microsoft.Graph.Models.Message message)
        {
            return new MailMessage(
                    message.Id,
                    message.Subject,
                    new Models.Recipient(message.Sender?.EmailAddress?.Address ?? string.Empty, message.Sender?.EmailAddress?.Name ?? string.Empty),
                    message.ToRecipients.Select(recipient => new Models.Recipient(recipient.EmailAddress.Address, recipient.EmailAddress.Name)),
                    message.CcRecipients.Select(recipient => new Models.Recipient(recipient.EmailAddress.Address, recipient.EmailAddress.Name)),
                    message.BodyPreview,
                    new MailMessageContent(message.Body.Content),
                    message.ParentFolderId);
        }
    }
}
