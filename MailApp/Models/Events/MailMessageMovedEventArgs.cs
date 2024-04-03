using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailApp.Models.Events
{
    public class MailMessageMovedEventArgs : EventArgs
    {
        public MailMessageMovedEventArgs(MailMessage mailMessage, string originFolderId, string newFolderId)
        {
            MailMessage = mailMessage;
            OriginFolderId = originFolderId;
            NewFolderId = newFolderId;
        }

        public MailMessage MailMessage { get; }
        public string OriginFolderId { get; }
        public string NewFolderId { get; }
    }
}
