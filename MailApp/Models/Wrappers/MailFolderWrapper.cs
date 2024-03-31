using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

#nullable enable

namespace MailApp.Models
{
    public class MailFolderWrapper
    {
        public MailFolderWrapper(MailFolder mailFolder)
        {
            MailFolder = mailFolder;
        }

        public MailFolder MailFolder { get; }
        public ObservableCollection<MailFolderWrapper> SubFolders { get; } = new();

        static bool FindParentFolder(IEnumerable collection, MailFolder query, out MailFolderWrapper parentFolderWrapper)
        {
            foreach (var item in collection)
            {
                if (item is not MailFolderWrapper folderWrapper)
                    continue;

                if (folderWrapper.MailFolder.Id == query.ParentFolderId)
                {
                    parentFolderWrapper = folderWrapper;
                    return true;
                }

                if (FindParentFolder(folderWrapper.SubFolders, query, out var foundParentFolderWrapper))
                {
                    parentFolderWrapper = foundParentFolderWrapper;
                    return true;
                }
            }

            parentFolderWrapper = null!;
            return false;
        }

        /// <summary>
        /// 根据指定的所有 MailFolder, 创建 MailFolderWrapper, 构建父子关系, 并填入 collection 中
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="folders"></param>
        public static void PopulateCollection(IList collection, MailFolder folder)
        {
            var folderWrapper = new MailFolderWrapper(folder);

            if (FindParentFolder(collection, folder, out var parentFolderWrapper))
            {
                parentFolderWrapper.SubFolders.Add(folderWrapper);
            }
            else
            {
                collection.Add(folderWrapper);
            }
        }
    }
}
