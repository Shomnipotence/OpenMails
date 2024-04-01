using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using MailApp.Models;
using MailApp.Services;

#nullable enable

namespace MailApp.ViewModels
{
    public partial class MailsPageViewModel : ObservableObject
    {
        readonly Dictionary<MailFolder, MailFolderMessageCollection> _cachedFolderMessages = new();

        [ObservableProperty]
        private IMailService? _mailService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Messages))]
        private MailFolder? _folder;

        [ObservableProperty]
        private MailMessage? _selectedMessage;

        public MailFolderMessageCollection? Messages
        {
            get
            {
                if (MailService is null || Folder is null)
                    return null;

                if (!_cachedFolderMessages.TryGetValue(Folder, out var folderMessages))
                {
                    folderMessages = _cachedFolderMessages[Folder] = new(MailService, Folder);
                    _ = folderMessages.LoadMoreItemsAsync(18);
                }

                return folderMessages;
            }
        }

        public void ClearCache()
        {
            _cachedFolderMessages.Clear();
        }
    }
}
