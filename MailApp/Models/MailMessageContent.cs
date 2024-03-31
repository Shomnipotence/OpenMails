namespace MailApp.Models
{
    public class MailMessageContent
    {
        public MailMessageContent(
            string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}
