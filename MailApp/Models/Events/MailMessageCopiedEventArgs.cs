using System;

namespace MailApp.Models.Events
{
    public class MailMessageCopiedEventArgs : EventArgs
    {
        public MailMessageCopiedEventArgs(MailMessage originMailMessage, MailMessage newMailMessage)
        {
            OriginMailMessage = originMailMessage;
            NewMailMessage = newMailMessage;
        }

        public MailMessage OriginMailMessage { get; }
        public MailMessage NewMailMessage { get; }
    }
}
