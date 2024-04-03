using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using MailApp.Models;
using Windows.Storage;

namespace MailApp.Services
{
    public class CacheService
    {
        private const string CacheFileName = "Cache.db";
        private const string MailCacheFolderName = "MailCache";
        private const string MailCommonFoldersCollectionName = "CommonFolders";
        private const string MailCustomFoldersCollectionName = "CustomFolders";
        private const string MailMessagesCollectionName = "Messages";

        private readonly Dictionary<IMailService, MailServiceCache> _cachedMailServiceDatabases = new();

        private LiteDatabase _liteDatabase;

        public CacheService()
        {

        }

        public async Task<MailServiceCache> GetMailServiceCacheAsync(IMailService service)
        {
            if (_cachedMailServiceDatabases.TryGetValue(service, out var existedCache))
            {
                return existedCache;
            }

            var mailCacheFolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync(MailCacheFolderName, CreationCollisionOption.OpenIfExists);
            var targetCacheFileStream = await mailCacheFolder.OpenStreamForWriteAsync($"{service.ServiceName}-{service.Address}.db", CreationCollisionOption.OpenIfExists);
            var database = new LiteDatabase(targetCacheFileStream);
            var cache = new MailServiceCache(service, database);

            _cachedMailServiceDatabases[service] = cache;

            return cache;
        }

        public void CloseAll()
        {
            foreach (var cache in _cachedMailServiceDatabases.Values)
                cache.Database.Dispose();

            _cachedMailServiceDatabases.Clear();
        }

        public class MailServiceCache
        {
            public MailServiceCache(IMailService mailService, ILiteDatabase database)
            {
                MailService = mailService;
                Database = database;

                _commonFolderCollection = Database.GetCollection<MailFolder>(MailCommonFoldersCollectionName);
                _customFolderCollection = Database.GetCollection<MailFolder>(MailCustomFoldersCollectionName);
                _messageCollection = Database.GetCollection<MailMessage>(MailMessagesCollectionName);
            }

            public IMailService MailService { get; }
            public ILiteDatabase Database { get; }

            private ILiteCollection<MailFolder> _commonFolderCollection;
            private ILiteCollection<MailFolder> _customFolderCollection;
            private ILiteCollection<MailMessage> _messageCollection;

            public IEnumerable<MailFolder> CommonFolders
                => _commonFolderCollection.Query().ToEnumerable();

            public IEnumerable<MailFolder> CustomFolders
                => _customFolderCollection.Query().ToEnumerable();

            public void SaveCommonFolders(IEnumerable<MailFolder> folders)
            {
                _commonFolderCollection.DeleteAll();
                _commonFolderCollection.Insert(folders);
            }

            public void SaveCustomFolders(IEnumerable<MailFolder> folders)
            {
                _customFolderCollection.DeleteAll();
                _customFolderCollection.Insert(folders);
            }
        }
    }
}
