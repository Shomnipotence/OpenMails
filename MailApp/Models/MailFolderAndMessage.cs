namespace MailApp.Models
{
    public record MailFolderAndMessage
    {
        public MailFolder? Folder { get; set; }
        public MailMessage? Message { get; set; }
    }
}
