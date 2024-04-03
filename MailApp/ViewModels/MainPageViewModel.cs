using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MailApp.Models;
using MailApp.Services;
using EleCho.Internationalization;
using System.Threading;
using Windows.Services.Maps;
using Microsoft.Extensions.DependencyInjection;
using MailApp.Views.Dialogs;
using System;
using Windows.UI.Xaml.Controls;
using MailApp.Extensions;

#nullable enable

namespace MailApp.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly CacheService _cacheService = App.Host.Services.GetRequiredService<CacheService>();
        private readonly ToastService _toastService = App.Host.Services.GetRequiredService<ToastService>();
        private readonly I18nStrings _i18nStrings = App.Host.Services.GetRequiredService<I18nStrings>();

        /// <summary>
        /// 邮箱服务与其所有邮箱文件夹
        /// </summary>
        private readonly Dictionary<IMailService, ObservableCollection<MailFolder>> _service2allMailFolderNodes = new();

        /// <summary>
        /// 已选择的邮箱服务
        /// </summary>
        [ObservableProperty]
        private IMailService? _selectedMailService = null;

        /// <summary>
        /// 左侧导航项
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<object>? _navigationViewItems;

        /// <summary>
        /// 已选中的导航项
        /// </summary>
        [ObservableProperty]
        private object? _selectedNavigationItem;

        /// <summary>
        /// 导航项是否展开
        /// </summary>
        [ObservableProperty]
        private bool _isNavigationViewPaneOpen = true;

        /// <summary>
        /// 当前所有邮件文件夹
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<MailFolder>? _currentAllMailFolders;

        private async Task CacheCurrentFoldersAsync()
        {
            if (SelectedMailService is null)
            {
                return;
            }

            var cache = await _cacheService.GetMailServiceCacheAsync(SelectedMailService);
            cache.SaveCommonFolders(CurrentAllMailFolders.Where(f => f.IsCommonFolder));
            cache.SaveCustomFolders(CurrentAllMailFolders.Where(f => !f.IsCommonFolder));
        }

        private async Task AddFolderToCurrentAsync(MailFolder folder)
        {
            if (CurrentAllMailFolders is null ||
                NavigationViewItems is null)
            {
                return;
            }

            CurrentAllMailFolders.Add(folder);
            var node = await TreeNode<MailFolder>.PopulateToCollectionAsync(NavigationViewItems, folder);

            await CacheCurrentFoldersAsync();
        }

        private async Task RemoveFolderToCurrentAsync(MailFolder folder)
        {
            // 从界面上移除文件夹
            if (NavigationViewItems is null ||
                CurrentAllMailFolders is null)
            {
                return;
            }

            var removedNode = TreeNode<MailFolder>.RemoveFromCollection(NavigationViewItems, folder);
            CurrentAllMailFolders.RemoveAny(existFolder => existFolder.Id == folder.Id);

            await CacheCurrentFoldersAsync();
        }

        /// <summary>
        /// 加载当前邮箱服务的文件夹 <br/>
        /// </summary>
        /// <param name="mailService"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task UpdateCurrentMailServiceFolders(CancellationToken cancellationToken)
        {
            // 加载当前邮箱服务的文件夹
            // 1. 如果已经加载过一次, 那么从内存中读取
            // 2. 如果是第一次加载, 也就是内存中没有缓存, 那么从硬盘中读取
            // 3. 通过邮箱服务请求所有文件夹, 并 "填充" 入刚刚的缓存列表
            //
            // 当来自网络的文件夹加载完毕时, 不会直接替换导航项
            // 而是增量更新, 有删减时移除, 有新增时加入, 如果文件夹被移动
            // 那么也会移动导航项的位置
            // (这部分填充逻辑在 TreeNode<T> 中)


            // 确保当前选中了邮箱服务
            if (SelectedMailService is not IMailService mailService)
            {
                return;
            }

            // 读取已经加载好的邮件文件夹
            if (!_service2allMailFolderNodes.TryGetValue(mailService, out var currentAllMailFolders))
            {
                var cache = await _cacheService.GetMailServiceCacheAsync(mailService);
                currentAllMailFolders = _service2allMailFolderNodes[mailService] = new();

                // 第一次会从缓存中读取数据
                foreach (var mailFolder in cache.CommonFolders)
                {
                    currentAllMailFolders.Add(mailFolder);
                }

                foreach (var mailFolder in cache.CustomFolders)
                {
                    currentAllMailFolders.Add(mailFolder);
                }
            }

            NavigationViewItems = new();
            CurrentAllMailFolders = currentAllMailFolders;

            // 先添加一条分割线 (邮箱列表与文件夹列表之间的)
            NavigationViewItems.Add(new NavigationViewItemSeparator());

            // 通过加载好的邮件文件夹, 构建导航项
            if (currentAllMailFolders.Count != 0)
            {
                foreach (var mailFolder in currentAllMailFolders.Where(f => f.IsCommonFolder))
                {
                    await TreeNode<MailFolder>.PopulateToCollectionAsync(NavigationViewItems, mailFolder);
                }

                NavigationViewItems.Add(new NavigationViewItemSeparator());

                foreach (var mailFolder in currentAllMailFolders.Where(f => !f.IsCommonFolder))
                {
                    await TreeNode<MailFolder>.PopulateToCollectionAsync(NavigationViewItems, mailFolder);
                }
            }

            // 选中第一个
            if (SelectedNavigationItem is null)
            {
                SelectedNavigationItem = NavigationViewItems
                    .OfType<TreeNode<MailFolder>>()
                    .FirstOrDefault();
            }

            // 加载远程邮件文件夹
            List<MailFolder> remoteFolders = new();

            await foreach (var commonMailFolder in mailService.GetAllCommonFoldersAsync(cancellationToken))
            {
                remoteFolders.Add(commonMailFolder);
                await TreeNode<MailFolder>.PopulateToCollectionAsync(NavigationViewItems, commonMailFolder);
            }

            if (currentAllMailFolders.Count == 0)
            {
                // 如果在这之前, 文件夹列表是空的, 那么就没有构建导航项,
                // 也就是没有分割线, 需要添加
                NavigationViewItems.Add(new NavigationViewItemSeparator());
            }

            await foreach (var customMailFolder in mailService.GetAllCustomFoldersAsync(cancellationToken))
            {
                remoteFolders.Add(customMailFolder);
                await TreeNode<MailFolder>.PopulateToCollectionAsync(NavigationViewItems, customMailFolder);
            }

            List<MailFolder> cacheFoldersToRemove = new();

            // 删去不存在的文件夹
            foreach (var cachedFolder in CurrentAllMailFolders)
            {
                if (remoteFolders.Any(remoteFolder => remoteFolder.Id == cachedFolder.Id))
                {
                    continue;
                }

                cacheFoldersToRemove.Add(cachedFolder);
                TreeNode<MailFolder>.RemoveFromCollection(NavigationViewItems, cachedFolder);
            }

            // 移除
            foreach (var cacheFolderToRemove in cacheFoldersToRemove)
            {
                CurrentAllMailFolders.Remove(cacheFolderToRemove);
            }

            // 更新已有或者添加新增文件夹
            foreach (var remoteFolder in remoteFolders)
            {
                var existedCachedFolder = CurrentAllMailFolders.FirstOrDefault(folder => folder.Id == remoteFolder.Id);

                if (existedCachedFolder is not null)
                {
                    // 已有, 更新值
                    MailFolder.Populate(remoteFolder, existedCachedFolder);
                }
                else
                {
                    // 添加
                    CurrentAllMailFolders.Add(remoteFolder);
                }
            }

            // 重新选中第一个 (因为可能会将用户选中的移除)
            if (SelectedNavigationItem is null)
            {
                SelectedNavigationItem = NavigationViewItems
                    .OfType<TreeNode<MailFolder>>()
                    .FirstOrDefault();
            }

            await CacheCurrentFoldersAsync();
        }

        /// <summary>
        /// Load folders after selected mail service changed
        /// </summary>
        /// <param name="mailService"></param>
        async partial void OnSelectedMailServiceChanged(IMailService? mailService)
        {
            if (mailService is null)
                return;

            try
            {
                // 更新当前邮箱服务的文件夹
                await UpdateCurrentMailServiceFolders(default);
            }
            catch
            {

            }
        }

        [RelayCommand]
        public void ToggleNavigationViewPane()
        {
            IsNavigationViewPaneOpen ^= true;
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [RelayCommand]
        public async Task CreateFolderAsync(CancellationToken cancellationToken)
        {
            if (SelectedMailService is null)
            {
                _toastService.ToastError(_i18nStrings.StringNoMailServiceWasSelected);
                return;
            }

            var strings = App.Host.Services
                .GetRequiredService<I18nStrings>();

            TextInputDialog dialog = new()
            {
                Title = strings.StringCreateSubFolder,
                PrimaryButtonText = strings.StringOk,
                CloseButtonText = strings.StringCancel,
            };

            var dialogResult = await dialog.ShowAsync();

            if (dialogResult != ContentDialogResult.Primary)
            {
                // 用户取消了
                return;
            }

            try
            {
                var newFolder = await SelectedMailService.CreateFolderAsync(dialog.Text, cancellationToken);
                await AddFolderToCurrentAsync(newFolder);
            }
            catch
            {
                _toastService.ToastError(_i18nStrings.StringFailedToCreate);
            }
        }

        /// <summary>
        /// 创建子文件夹
        /// </summary>
        /// <param name="folder">要创建子文件夹的文件夹</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [RelayCommand]
        public async Task CreateSubFolderAsync(MailFolder folder, CancellationToken cancellationToken)
        {
            if (SelectedMailService is null)
            {
                _toastService.ToastError(_i18nStrings.StringNoMailServiceWasSelected);
                return;
            }

            var strings = App.Host.Services
                .GetRequiredService<I18nStrings>();

            TextInputDialog dialog = new()
            {
                Title = strings.StringCreateSubFolder,
                PrimaryButtonText = strings.StringOk,
                CloseButtonText = strings.StringCancel,
            };

            var dialogResult = await dialog.ShowAsync();

            if (dialogResult != ContentDialogResult.Primary)
            {
                // 用户取消了
                return;
            }

            try
            {
                var newFolder = await SelectedMailService.CreateSubFolderAsync(folder, dialog.Text, cancellationToken);
                await AddFolderToCurrentAsync(newFolder);
            }
            catch
            {
                _toastService.ToastError(_i18nStrings.StringFailedToCreate);
            }
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="folder">要删除的文件夹</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [RelayCommand]
        public async Task DeleteFolderAsync(MailFolder folder, CancellationToken cancellationToken)
        {
            if (SelectedMailService is null)
            {
                _toastService.ToastError(_i18nStrings.StringNoMailServiceWasSelected);
                return;
            }

            try
            {
                // 使用服务删除文件夹
                await SelectedMailService.DeleteFolderAsync(folder, cancellationToken);
                await RemoveFolderToCurrentAsync(folder);
            }
            catch
            {
                _toastService.ToastError(_i18nStrings.StringFailedToDelete);
            }
        }
    }
}