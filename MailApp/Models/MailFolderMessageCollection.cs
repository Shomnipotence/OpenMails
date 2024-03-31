using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MailApp.Services;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

#nullable enable

namespace MailApp.Models
{
    /// <summary>
    /// 支持增量加载的邮箱邮件集合, 供 UI 绑定使用
    /// </summary>
    public class MailFolderMessageCollection : ObservableCollection<MailMessage>, ISupportIncrementalLoading
    {
        bool _hasMoreItems = true;

        public MailFolderMessageCollection(IMailService mailService, MailFolder folder)
        {
            MailService = mailService;
            Folder = folder;
        }

        private async Task<LoadMoreItemsResult> LoadMessagesAsync(int take)
        {
            int skip = Count;
            int loadedCount = 0;
            try
            {
                await foreach (var message in MailService.GetMessagesInFolder(Folder, skip, take, default))
                {
                    Add(message);
                    loadedCount++;
                }

                if (loadedCount < take)
                    _hasMoreItems = false;

                return new LoadMoreItemsResult()
                {
                    Count = (uint)loadedCount,
                };
            }
            catch (Exception)
            {
                return new LoadMoreItemsResult()
                {
                    Count = 0
                };
            }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return LoadMessagesAsync((int)count).AsAsyncOperation();
        }

        public bool HasMoreItems => _hasMoreItems;

        public IMailService MailService { get; }
        public MailFolder Folder { get; }
    }
}
