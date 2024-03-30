using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using OpenMails.Models;
using OpenMails.Services;

#nullable enable

namespace OpenMails.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        // 存储 MailFolderWrapper 或 NavigationViewDivider
        readonly Dictionary<IMailService, ObservableCollection<object>> _cachedServiceNavigationItems = new();
        readonly Dictionary<MailFolder, MailFolderMessageCollection> _cachedFolderMessages = new();

        [ObservableProperty]
        IMailService? _selectedMailService = null;

        [ObservableProperty]
        ObservableCollection<MailMessage>? _currentFolderMailMessages;

        [ObservableProperty]
        ObservableCollection<object>? _navigationViewItems;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedFolderWrapper))]
        [NotifyPropertyChangedFor(nameof(SelectedFolder))]
        object? _selectedNavigationItem;

        [ObservableProperty]
        MailMessage? _selectedMessage;

        public MailFolderWrapper? SelectedFolderWrapper => _selectedNavigationItem as MailFolderWrapper;
        public MailFolder? SelectedFolder => (_selectedNavigationItem as MailFolderWrapper)?.MailFolder;

        public ObservableCollection<IMailService> MailServices { get; } = new();

        /// <summary>
        /// Load folders after selected mail service changed
        /// </summary>
        /// <param name="value"></param>
        async partial void OnSelectedMailServiceChanged(IMailService? value)
        {
            if (value is null)
                return;

            var loadingNavigationItems = new ObservableCollection<object>();
            if (_cachedServiceNavigationItems.TryGetValue(value, out var cachedNavigationItems))
                NavigationViewItems = cachedNavigationItems;

            var mailFolders = value.GetAllFoldersAsync(default);
            await MailFolderWrapper.PopulateCollectionAsync(loadingNavigationItems, mailFolders);
            NavigationViewItems = loadingNavigationItems;

            if (SelectedNavigationItem is MailFolderWrapper selectedMailFolder)
            {
                SelectedNavigationItem = loadingNavigationItems
                    .OfType<MailFolderWrapper>()
                    .FirstOrDefault(folder => folder.MailFolder.Id == selectedMailFolder.MailFolder.Id);
            }
            else if (SelectedNavigationItem is null)
            {
                // select the first folder
                SelectedNavigationItem = loadingNavigationItems
                    .OfType<MailFolderWrapper>()
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Load messages after selected mail folder changed
        /// </summary>
        /// <param name="value"></param>
        partial void OnSelectedNavigationItemChanged(object? value)
        {
            if (value is not MailFolderWrapper folderWrapper)
                return;
            if (SelectedMailService is null)
                return;

            if (!_cachedFolderMessages.TryGetValue(folderWrapper.MailFolder, out var folderMessageCollection))
            {
                folderMessageCollection = _cachedFolderMessages[folderWrapper.MailFolder] = new(SelectedMailService, folderWrapper.MailFolder);
                _ = folderMessageCollection.LoadMoreItemsAsync(18);
            }

            CurrentFolderMailMessages = folderMessageCollection;
        }

        partial void OnSelectedMessageChanged(MailMessage? value)
        {

        }
    }
}