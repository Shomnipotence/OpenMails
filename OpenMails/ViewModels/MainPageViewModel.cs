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
        readonly Dictionary<IMailService, ObservableCollection<object>> _cachedNavigationItems = new();
        readonly Dictionary<MailFolder, MailFolderMessageCollection> _cachedFolderMessages = new();

        [ObservableProperty]
        IMailService? _selectedMailService = null;

        [ObservableProperty]
        ObservableCollection<MailMessage>? _currentFolderMailMessages;

        [ObservableProperty]
        ObservableCollection<object>? _navigationViewItems;

        [ObservableProperty]
        MailFolder? _selectedFolder;

        [ObservableProperty]
        MailMessage? _selectedMessage;

        public ObservableCollection<IMailService> MailServices { get; } = new();

        /// <summary>
        /// Load folders after selected mail service changed
        /// </summary>
        /// <param name="value"></param>
        async partial void OnSelectedMailServiceChanged(IMailService? value)
        {
            if (value is null)
                return;

            if (!_cachedNavigationItems.TryGetValue(value, out var navigationViewItems))
                navigationViewItems = _cachedNavigationItems[value] = new();
            NavigationViewItems = navigationViewItems;

            // load all folders
            await foreach (var folder in value.GetAllFoldersAsync(default))
                navigationViewItems.Add(folder);

            if (SelectedFolder == null)
            {
                // select the first folder
                SelectedFolder = navigationViewItems
                    .OfType<MailFolder>()
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Load messages after selected mail folder changed
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        partial void OnSelectedFolderChanged(MailFolder? value)
        {
            if (value is not MailFolder folder)
                return;
            if (SelectedMailService is null)
                return;

            if (!_cachedFolderMessages.TryGetValue(folder, out var folderMessageCollection))
            {
                folderMessageCollection = _cachedFolderMessages[folder] = new(SelectedMailService, folder);
                _ = folderMessageCollection.LoadMoreItemsAsync(18);
            }

            CurrentFolderMailMessages = folderMessageCollection;
        }

        partial void OnSelectedMessageChanged(MailMessage? value)
        {

        }
    }
}