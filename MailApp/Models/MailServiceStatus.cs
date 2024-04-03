using System.Collections.Generic;
using MailApp.Services;

namespace MailApp.Models
{
    public record MailServiceStatus
    {
        public MailServiceStatus(IMailService mailService, IList<MailFolder> allFolders, MailFolder selectedFolder)
        {
            MailService = mailService;
            AllFolders = allFolders;
            SelectedFolder = selectedFolder;
        }

        public IMailService MailService { get; }
        public IEnumerable<MailFolder> AllFolders { get; }
        public MailFolder SelectedFolder { get; }
    }
}
