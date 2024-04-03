using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MailApp.Models;
using MailApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

#nullable enable

namespace MailApp.ViewModels
{
    public partial class MailsPageViewModel : ObservableObject
    {
        readonly ToastService _toastService = App.Host.Services.GetRequiredService<ToastService>();
        readonly I18nStrings _i18nStrings = App.Host.Services.GetRequiredService<I18nStrings>();

        readonly Dictionary<MailFolder, MailFolderMessageCollection> _cachedFolderMessages = new();

        [ObservableProperty]
        private IMailService? _mailService;

        [ObservableProperty]
        private IEnumerable<MailFolder>? _allFolders;

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


        //public IEnumerable<MenuFlyoutItemBase> MenuFlyoutSubItemsOfMoveToCommand
        //{
        //    get
        //    {
        //        if (AllFolders is null)
        //            yield break;

        //        foreach (var folderNode in AllFolders)
        //        {
        //            yield return CreateMenuFlyoutSubItemOfMoveToCommand(folderNode);
        //        }
        //    }
        //}

        public void ClearCache()
        {
            _cachedFolderMessages.Clear();
        }

        public void ClearFolderCache(string folderId)
        {
            var folder = _cachedFolderMessages.Keys.FirstOrDefault(f => f.Id == folderId);
            if (folder is not null)
                _cachedFolderMessages.Remove(folder);
        }

        [RelayCommand]
        public async Task DeleteMessageAsync(MailMessage message, CancellationToken cancellationToken)
        {
            if (MailService is null)
            {
                _toastService.ToastError(_i18nStrings.StringNoMailServiceWasSelected);
                return;
            }

            try
            {
                await MailService.DeleteMessageAsync(message, cancellationToken);

                if (Messages is not null)
                {
                    Messages.Remove(message);
                }

                if (SelectedMessage == message)
                {
                    SelectedMessage = null;
                }
            }
            catch
            {
                _toastService.ToastError(_i18nStrings.StringFailedToDelete);
            }
        }

        [RelayCommand]
        public async Task ArchiveMessageAsync(MailMessage message, CancellationToken cancellationToken)
        {
            if (MailService is null)
            {
                _toastService.ToastError(_i18nStrings.StringNoMailServiceWasSelected);
                return;
            }

            try
            {
                string containingFolderIdBefore = message.ContainingFolderId;
                await MailService.ArchiveMessageAsync(message, cancellationToken);

                if (Messages is not null)
                {
                    Messages.Remove(message);
                }

                if (SelectedMessage == message)
                {
                    SelectedMessage = null;
                }

                if (message.ContainingFolderId != containingFolderIdBefore)
                {
                    var cachedfolder = _cachedFolderMessages.Keys
                        .FirstOrDefault(folder => folder.Id == message.ContainingFolderId);

                    if (cachedfolder is not null)
                    {
                        _cachedFolderMessages.Remove(cachedfolder);
                    }
                }
            }
            catch
            {
                _toastService.ToastError(_i18nStrings.StringFailedToDelete);
            }
        }

        [RelayCommand]
        public async Task MoveMessageAsync(MailMessage message, CancellationToken cancellationToken)
        {
            if (MailService is null)
            {
                _toastService.ToastError(_i18nStrings.StringNoMailServiceWasSelected);
                return;
            }

            try
            {
                string containingFolderIdBefore = message.ContainingFolderId;
                await MailService.ArchiveMessageAsync(message, cancellationToken);

                if (Messages is not null)
                {
                    Messages.Remove(message);
                }

                if (SelectedMessage == message)
                {
                    SelectedMessage = null;
                }

                if (message.ContainingFolderId != containingFolderIdBefore)
                {
                    var cachedfolder = _cachedFolderMessages.Keys
                        .FirstOrDefault(folder => folder.Id == message.ContainingFolderId);

                    if (cachedfolder is not null)
                    {
                        _cachedFolderMessages.Remove(cachedfolder);
                    }
                }
            }
            catch
            {
                _toastService.ToastError(_i18nStrings.StringFailedToDelete);
            }
        }
    }
}
