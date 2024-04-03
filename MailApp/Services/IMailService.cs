using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MailApp.Models;
using Windows.UI.Xaml.Media;

#nullable enable

namespace MailApp.Services
{
    /// <summary>
    /// 邮箱服务 <br/>
    /// 除了 "获取所有文件夹" 与 "获取所有邮件" 的方法, 其他方法不标记 "Recursive" 均为非递归方法
    /// </summary>
    public interface IMailService
    {
        public IMailAuthService AuthService { get; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 用户地址
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public Task<ImageSource?> GetAvatarAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 获取所有文件夹, 不论层级, 不论父子关系
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IAsyncEnumerable<MailFolder> GetAllFoldersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取所有常规文件夹(自带文件夹), 不论层级, 不论父子关系
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IAsyncEnumerable<MailFolder> GetAllCommonFoldersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取所有自定义文件夹(用户自己创建的文件夹), 不论层级, 不论父子关系
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IAsyncEnumerable<MailFolder> GetAllCustomFoldersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取所有邮件, 不论这个邮件处于哪个文件夹
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IAsyncEnumerable<MailMessage> GetAllMessagesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取所有根文件夹
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IAsyncEnumerable<MailFolder> GetAllRootFoldersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取处于指定文件夹中的所有子文件夹 (不递归)
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IAsyncEnumerable<MailFolder> GetAllFoldersInFolderAsync(MailFolder folder, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取处于指定文件夹中的所有子文件夹, 以及其子文件夹的所有子文件夹 (递归)
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IAsyncEnumerable<MailFolder> RecursiveGetAllFoldersInFolder(MailFolder folder, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取目录中的所有邮件
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IAsyncEnumerable<MailMessage> GetAllMessagesInFolderAsync(MailFolder folder, CancellationToken cancellationToken = default);

        /// <summary>
        /// 查询目录中的所有邮件
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IAsyncEnumerable<MailMessage> QueryAllMessagesInFolderAsync(MailFolder folder, MailMessageQuery query, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取目录中的邮件
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IAsyncEnumerable<MailMessage> GetMessagesInFolder(MailFolder folder, int skip, int take, CancellationToken cancellationToken = default);

        /// <summary>
        /// 查询目录中的邮件
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="query"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IAsyncEnumerable<MailMessage> QueryMessagesInFolder(MailFolder folder, MailMessageQuery query, int skip, int take, CancellationToken cancellationToken = default);

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MailFolder> CreateFolderAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// 创建子文件夹
        /// </summary>
        /// <param name="parentFolder"></param>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MailFolder> CreateSubFolderAsync(MailFolder parentFolder, string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task DeleteFolderAsync(MailFolder folder, CancellationToken cancellationToken = default);

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task DeleteMessageAsync(MailMessage message, CancellationToken cancellationToken = default);

        /// <summary>
        /// 存档消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task ArchiveMessageAsync(MailMessage message, CancellationToken cancellationToken = default);

        /// <summary>
        /// 将消息移动到指定目录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="destinationFolder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task MoveMessageAsync(MailMessage message, MailFolder destinationFolder, CancellationToken cancellationToken = default);

        /// <summary>
        /// 将消息移动到指定目录
        /// </summary>
        /// <param name="message"></param>
        /// <param name="destinationFolder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MailMessage> CopyMessageAsync(MailMessage message, MailFolder destinationFolder, CancellationToken cancellationToken = default);
    }
}
