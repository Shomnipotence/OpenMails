using CommunityToolkit.Mvvm.ComponentModel;

#nullable enable

namespace OpenMails.Models
{
    public partial class MailMessageWrapper : ObservableObject
    {
        [ObservableProperty]
        private bool _isEditing;

        public MailMessageWrapper(MailMessage mailMessage)
        {

        }
    }
}
