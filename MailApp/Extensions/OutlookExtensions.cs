using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailApp.Enums;
using MailApp.Models;
using Microsoft.Kiota.Abstractions;

namespace MailApp.Extensions
{

    /// <summary>
    /// Outlook 所使用的拓展函数
    /// </summary>
    internal static class OutlookExtensions
    {
        public static void PopulateToParameters(
            this MailMessageQuery query,
            RequestConfiguration<Microsoft.Graph.Me.MailFolders.Item.Messages.MessagesRequestBuilder.MessagesRequestBuilderGetQueryParameters> parameters)
        {
            // TODO: 实现抽象查询到 Graph API 参数的填充
        }

        public static void PopulateToParameters(
            this MailMessageQuery query, 
            RequestConfiguration<Microsoft.Graph.Me.Messages.MessagesRequestBuilder.MessagesRequestBuilderGetQueryParameters> parameters)
        {
            // TODO: 实现抽象查询到 Graph API 参数的填充
        }

        /// <summary>
        /// 将 Outlook Graph 邮箱文件夹转换为当前项目抽象的文件夹
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static Models.MailFolder ToAppMailFolder(this Microsoft.Graph.Models.MailFolder folder, MailFolderIcon icon = MailFolderIcon.Default, bool isCommonFolder = false)
        {
            return new MailApp.Models.MailFolder(folder.Id, folder.DisplayName, folder.DisplayName, icon, folder.ParentFolderId, isCommonFolder, (folder.TotalItemCount ?? 0) == 0);
        }

        /// <summary>
        /// 将 Outlook Graph 邮箱邮件转换为当前项目抽象的邮件
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static MailMessage ToAppMailMessage(this Microsoft.Graph.Models.Message message)
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
