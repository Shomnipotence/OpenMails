using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenMails.Models;
using Windows.UI.Xaml.Media;

namespace OpenMails.Services
{
    /// <summary>
    /// 邮箱服务 <br/>
    /// 除了 "获取所有文件夹" 与 "获取所有邮件" 的方法, 其他方法不标记 "Recursive" 均为非递归方法
    /// </summary>
    public interface IMailService
    {
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
        public ImageSource Avatar { get; }

        /// <summary>
        /// 获取所有文件夹, 不论层级, 不论父子关系
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IAsyncEnumerable<MailFolder> GetAllFoldersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取所有邮件, 不论这个邮件处于哪个文件夹
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IAsyncEnumerable<MailMessage> GetAllMessagesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取所有根目录的文件夹
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
        /// 获取目录中的邮件 (根据指定参数)
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IAsyncEnumerable<MailMessage> GetMessagesInFolder(MailFolder folder, int skip, int take, CancellationToken cancellationToken = default);
    }
}
