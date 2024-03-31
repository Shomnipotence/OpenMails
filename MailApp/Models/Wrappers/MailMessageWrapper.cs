using CommunityToolkit.Mvvm.ComponentModel;

#nullable enable

namespace MailApp.Models
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
