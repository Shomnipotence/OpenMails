using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MailApp.Services
{
    public partial class ApplicationGlobalData : ObservableObject
    {
        public ObservableCollection<IMailService> MailServices { get; } = new();
    }
}
