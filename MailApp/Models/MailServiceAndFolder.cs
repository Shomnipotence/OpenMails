using MailApp.Services;

namespace MailApp.Models
{
    public record MailServiceAndFolder
    { 
        public MailServiceAndFolder(IMailService service, MailFolder folder)
        {
            Service = service;
            Folder = folder;
        }

        public IMailService Service { get; }
        public MailFolder Folder { get; }
    }
}
