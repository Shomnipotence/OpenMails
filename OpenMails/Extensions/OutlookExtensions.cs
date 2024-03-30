using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenMails.Models;

namespace OpenMails.Extensions
{
    /// <summary>
    /// Outlook 所使用的拓展函数
    /// </summary>
    internal static class OutlookExtensions
    {
        /// <summary>
        /// 将 Outlook Graph 邮箱文件夹转换为当前项目抽象的文件夹
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static Models.MailFolder ToCommonMailFolder(this Microsoft.Graph.Models.MailFolder folder)
        {
            return new OpenMails.Models.MailFolder(folder.Id, folder.DisplayName, folder.DisplayName, folder.ParentFolderId, (folder.TotalItemCount ?? 0) == 0);
        }

        /// <summary>
        /// 将 Outlook Graph 邮箱邮件转换为当前项目抽象的邮件
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
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
