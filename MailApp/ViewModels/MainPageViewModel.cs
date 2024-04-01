using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using MailApp.Models;
using MailApp.Services;

#nullable enable

namespace MailApp.ViewModels
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

        [ObservableProperty]
        bool _isNavigationViewPaneOpen = true;

        public MailFolderWrapper? SelectedFolderWrapper => SelectedNavigationItem as MailFolderWrapper;
        public MailFolder? SelectedFolder => (SelectedNavigationItem as MailFolderWrapper)?.MailFolder;

        /// <summary>
        /// Load folders after selected mail service changed
        /// </summary>
        /// <param name="value"></param>
        async partial void OnSelectedMailServiceChanged(IMailService? value)
        {
            if (value is null)
                return;

            try
            {
                var loadingNavigationItems = new ObservableCollection<object>();
                if (_cachedServiceNavigationItems.TryGetValue(value, out var cachedNavigationItems))
                    NavigationViewItems = cachedNavigationItems;
                else
                    NavigationViewItems = loadingNavigationItems;

                // add separator
                loadingNavigationItems.Add(new NavigationViewItemSeparator());

                // 加载文件夹
                await foreach (var mailFolder in value.GetAllCommonFoldersAsync(default))
                {
                    MailFolderWrapper.PopulateCollection(loadingNavigationItems, mailFolder);

                    // 如果当前界面上显示的导航项是正在加载的
                    if (NavigationViewItems == loadingNavigationItems)
                    {
                        if (SelectedNavigationItem is null)
                        {
                            await Task.Delay(1);

                            // 选择第一个文件夹
                            SelectedNavigationItem = loadingNavigationItems
                                .OfType<MailFolderWrapper>()
                                .FirstOrDefault();
                        }
                    }
                }

                // add separator
                loadingNavigationItems.Add(new NavigationViewItemSeparator());

                await foreach (var mailFolder in value.GetAllCustomFoldersAsync(default))
                {
                    MailFolderWrapper.PopulateCollection(loadingNavigationItems, mailFolder);
                }

                // 当前是否正在用加载完毕的项替换缓存好的项
                bool replaceNavigationViewItems = NavigationViewItems != loadingNavigationItems;
                NavigationViewItems = loadingNavigationItems;

                if (replaceNavigationViewItems)
                {
                    if (SelectedNavigationItem is MailFolderWrapper selectedMailFolder)
                    {
                        // 选择匹配的文件夹
                        SelectedNavigationItem = loadingNavigationItems
                            .OfType<MailFolderWrapper>()
                            .FirstOrDefault(folder => folder.MailFolder.Id == selectedMailFolder.MailFolder.Id);
                    }
                }
            }
            catch (System.OperationCanceledException)
            {
                // do nothing
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
                //_ = folderMessageCollection.LoadMoreItemsAsync(18);
            }

            CurrentFolderMailMessages = folderMessageCollection;
        }

        partial void OnSelectedMessageChanged(MailMessage? value)
        {

        }

        [RelayCommand]
        public void ToggleNavigationViewPane()
        {
            IsNavigationViewPaneOpen ^= true;
        }
    }
}