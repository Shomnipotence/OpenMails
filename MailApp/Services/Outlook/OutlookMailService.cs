using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using MailApp.Enums;
using MailApp.Extensions;
using MailApp.Models;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

#nullable enable

namespace MailApp.Services.Outlook
{
    public class OutlookMailService : IMailService
    {
        private static readonly string[] s_graphScoped
            = ["User.Read", "Mail.ReadWrite"];
        private static readonly string[] s_wellKnownFolderNames
            = ["Inbox", "Archive", "SentItems", "DeletedItems", "JunkEmail", "Drafts", "SyncIssues"];

        private GraphServiceClient _graphServiceClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken">access token from Microsoft Graph auth api</param>
        public OutlookMailService(OutlookAuthService authService, IAccount account, string accessToken)
        {
            var pca = PublicClientApplicationBuilder
                .Create(AppSecrets.MicrosoftGraphClientId)
                .WithTenantId(AppSecrets.MicrosoftGraphTenantId)
                .Build();

            var authProvider = new OutlookMailServiceAuthenticationProvider(accessToken);
            AuthService = authService;
            Account = account;
            _graphServiceClient = new GraphServiceClient(authProvider);
        }

        public IMailAuthService AuthService { get; }

        public string ServiceName => "Outlook";

        public string Name => Account.GetTenantProfiles().FirstOrDefault()?.ClaimsPrincipal?.FindFirst("name")?.Value ?? string.Empty;
        public string Address => Account.Username;

        public IAccount Account { get; }

        private MailFolderIcon GetIconFromWellKnownName(string folderName)
        {
            return folderName switch
            {
                "Inbox" => MailFolderIcon.Inbox,
                "Archive" => MailFolderIcon.Archive,
                "SentItems" => MailFolderIcon.SentItems,
                "DeletedItems" => MailFolderIcon.DeletedItems,
                "JunkEmail" => MailFolderIcon.JunkEmail,
                "Drafts" => MailFolderIcon.Drafts,
                "SyncIssues" => MailFolderIcon.SyncIssues,
                _ => MailFolderIcon.Default
            };
        }

        /// <summary>
        /// 获取所有文件夹, 无论层级
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<Models.MailFolder> GetAllFoldersAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            List<Models.MailFolder> rootFolders = new();
            await foreach (var rootFolder in GetAllRootFoldersAsync(cancellationToken))
            {
                rootFolders.Add(rootFolder);
                yield return rootFolder;
            }

            List<IAsyncEnumerable<Models.MailFolder>> recursiveSubFoldersEnumerables = new();
            foreach (var rootFolder in rootFolders)
            {
                recursiveSubFoldersEnumerables.Add(RecursiveGetAllFoldersInFolder(rootFolder, cancellationToken));
            }

            foreach (var recursiveSubFolders in recursiveSubFoldersEnumerables)
            {
                await foreach (var subFolder in recursiveSubFolders)
                {
                    yield return subFolder;
                }
            }
        }

        /// <summary>
        /// 获取所有公共文件夹 (非用户自建文件夹)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<Models.MailFolder> GetAllCommonFoldersAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            List<Task<Microsoft.Graph.Models.MailFolder>> allFolderTasks = new();

            foreach (var folderName in s_wellKnownFolderNames)
            {
                allFolderTasks.Add(_graphServiceClient.Me.MailFolders[folderName].GetAsync(parameters => { }, cancellationToken));
            }

            for (int i = 0; i < allFolderTasks.Count; i++)
            {
                var folderTask = allFolderTasks[i];
                var wellKnownName = s_wellKnownFolderNames[i];

                await folderTask;
                yield return folderTask.Result.ToAppMailFolder(GetIconFromWellKnownName(wellKnownName), true);
            }
        }

        /// <summary>
        /// 获取所有自定义文件夹 (用户自建文件夹)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<Models.MailFolder> GetAllCustomFoldersAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var allCommonFolders = await GetAllCommonFoldersAsync(cancellationToken).ToArrayAsync();
            await foreach (var mailFolder in GetAllFoldersAsync(cancellationToken))
            {
                if (allCommonFolders.Any(commonMailFolder => commonMailFolder.Id == mailFolder.Id))
                    continue;

                yield return mailFolder;
            }
        }


        /// <summary>
        /// 获取所有邮件
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<MailMessage> GetAllMessagesAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var response = await _graphServiceClient.Me.Messages.GetAsync(parameter => { }, cancellationToken);

            foreach (var message in response.Value)
                yield return message.ToAppMailMessage();
        }

        /// <summary>
        /// 获取所有根文件夹
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<Models.MailFolder> GetAllRootFoldersAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var response = await _graphServiceClient.Me.MailFolders.GetAsync(parameters =>
            {
                parameters.QueryParameters.IncludeHiddenFolders = "true";
            }, cancellationToken);

            foreach (var folder in response.Value)
                yield return folder.ToAppMailFolder();

            while (!string.IsNullOrEmpty(response.OdataNextLink))
            {
                response = await _graphServiceClient.Me.MailFolders
                    .WithUrl(response.OdataNextLink)
                    .GetAsync(cancellationToken: cancellationToken);

                foreach (var folder in response.Value)
                    yield return folder.ToAppMailFolder();
            }
        }

        /// <summary>
        /// 获取文件夹的所有子文件夹
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<Models.MailFolder> GetAllFoldersInFolderAsync(
            Models.MailFolder folder,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var response = await _graphServiceClient.Me.MailFolders[folder.Id].ChildFolders.GetAsync(parameters => { }, cancellationToken);

            foreach (var childFolder in response.Value)
                yield return childFolder.ToAppMailFolder();

            while (!string.IsNullOrEmpty(response.OdataNextLink))
            {
                response = await _graphServiceClient.Me.MailFolders
                    .WithUrl(response.OdataNextLink)
                    .GetAsync(cancellationToken: cancellationToken);

                foreach (var nextChildFolder in response.Value)
                    yield return nextChildFolder.ToAppMailFolder();
            }
        }

        /// <summary>
        /// 递归式的获取某文件夹下所有层级的所有文件夹
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<Models.MailFolder> RecursiveGetAllFoldersInFolder(
            Models.MailFolder folder,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            List<IAsyncEnumerable<Models.MailFolder>> recursiveSubFoldersEnumerables = new();
            await foreach (var childFolder in GetAllFoldersInFolderAsync(folder, cancellationToken))
            {
                yield return childFolder;

                recursiveSubFoldersEnumerables.Add(RecursiveGetAllFoldersInFolder(childFolder, cancellationToken));
            }

            foreach (var recursiveSubFolders in recursiveSubFoldersEnumerables)
            {
                await foreach (var deeperSubFolder in recursiveSubFolders)
                    yield return deeperSubFolder;
            }
        }

        /// <summary>
        /// 获取文件夹中的所有邮件
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<MailMessage> GetAllMessagesInFolderAsync(
            Models.MailFolder folder,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var response = await _graphServiceClient.Me.MailFolders[folder.Id].Messages.GetAsync(parameters => { }, cancellationToken);

            foreach (var message in response.Value)
                yield return message.ToAppMailMessage();

            while (!string.IsNullOrEmpty(response.OdataNextLink))
            {
                response = await _graphServiceClient.Me.Messages
                    .WithUrl(response.OdataNextLink)
                    .GetAsync(cancellationToken: cancellationToken);

                foreach (var message in response.Value)
                    yield return message.ToAppMailMessage();
            }
        }

        /// <summary>
        /// 获取文件夹中的邮件, 根据指定查询
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<MailMessage> GetMessagesInFolder(
            Models.MailFolder folder,
            int skip,
            int take,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var response = await _graphServiceClient.Me.MailFolders[folder.Id].Messages.GetAsync(parameters =>
            {
                parameters.QueryParameters.Skip = skip;
                parameters.QueryParameters.Top = take;
            }, cancellationToken);

            foreach (var message in response.Value)
                yield return message.ToAppMailMessage();
        }

        /// <summary>
        /// 查询目录中的所有邮件
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<MailMessage> QueryAllMessagesInFolderAsync(
            Models.MailFolder folder,
            MailMessageQuery query,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var response = await _graphServiceClient.Me.MailFolders[folder.Id].Messages.GetAsync(parameters =>
            {
                query.PopulateToParameters(parameters);
            }, cancellationToken);

            foreach (var message in response.Value)
                yield return message.ToAppMailMessage();

            while (!string.IsNullOrEmpty(response.OdataNextLink))
            {
                response = await _graphServiceClient.Me.Messages
                    .WithUrl(response.OdataNextLink)
                    .GetAsync(cancellationToken: cancellationToken);

                foreach (var message in response.Value)
                    yield return message.ToAppMailMessage();
            }
        }

        /// <summary>
        /// 查询目录中的邮件
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="query"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<MailMessage> QueryMessagesInFolder(
            Models.MailFolder folder,
            MailMessageQuery query,
            int skip,
            int take,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var response = await _graphServiceClient.Me.MailFolders[folder.Id].Messages.GetAsync(parameters =>
            {
                query.PopulateToParameters(parameters);
                parameters.QueryParameters.Skip = skip;
                parameters.QueryParameters.Top = take;
            }, cancellationToken);

            foreach (var message in response.Value)
                yield return message.ToAppMailMessage();
        }

        public async Task<ImageSource?> GetAvatarAsync(CancellationToken cancellationToken)
        {
            try
            {
                var photo = await _graphServiceClient.Me.Photo.Content.GetAsync();
                var bitmap = new BitmapImage();
                MemoryStream ms = new();
                await photo.CopyToAsync(ms);
                ms.Position = 0;
                await bitmap.SetSourceAsync(ms.AsRandomAccessStream());
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Models.MailFolder> CreateFolderAsync(string name, CancellationToken cancellationToken = default)
        {
            var result = await _graphServiceClient.Me.MailFolders.PostAsync(
                new Microsoft.Graph.Models.MailFolder()
                {
                    DisplayName = name,
                }, cancellationToken: cancellationToken);

            return result.ToAppMailFolder();
        }

        public async Task<Models.MailFolder> CreateSubFolderAsync(Models.MailFolder parentFolder, string name, CancellationToken cancellationToken = default)
        {
            var result = await _graphServiceClient.Me.MailFolders[parentFolder.Id].ChildFolders.PostAsync(
                new Microsoft.Graph.Models.MailFolder()
                {
                    DisplayName = name
                }, cancellationToken: cancellationToken);

            return result.ToAppMailFolder();
        }

        public async Task DeleteFolderAsync(Models.MailFolder folder, CancellationToken cancellationToken = default)
        {
            await _graphServiceClient.Me.MailFolders[folder.Id].DeleteAsync(cancellationToken: cancellationToken);
        }

        public async Task DeleteMessageAsync(MailMessage message, CancellationToken cancellationToken = default)
        {
            await _graphServiceClient.Me.Messages[message.Id].DeleteAsync(cancellationToken: cancellationToken);
        }

        public async Task ArchiveMessageAsync(MailMessage message, CancellationToken cancellationToken = default)
        {
            var result = await _graphServiceClient.Me.Messages[message.Id].Move.PostAsync(new Microsoft.Graph.Me.Messages.Item.Move.MovePostRequestBody()
            {
                DestinationId = "Archive"
            }, cancellationToken: cancellationToken);

            message.ContainingFolderId = result.ParentFolderId;
        }

        public async Task MoveMessageAsync(MailMessage message, Models.MailFolder destinationFolder, CancellationToken cancellationToken = default)
        {
            var result = await _graphServiceClient.Me.Messages[message.Id].Move.PostAsync(new Microsoft.Graph.Me.Messages.Item.Move.MovePostRequestBody()
            {
                DestinationId = destinationFolder.Id,
            }, cancellationToken: cancellationToken);

            message.ContainingFolderId = result.ParentFolderId;
        }

        public async Task<MailMessage> CopyMessageAsync(MailMessage message, Models.MailFolder destinationFolder, CancellationToken cancellationToken = default)
        {
            var result = await _graphServiceClient.Me.Messages[message.Id].Copy.PostAsync(new Microsoft.Graph.Me.Messages.Item.Copy.CopyPostRequestBody()
            {
                DestinationId = destinationFolder.Id,
            }, cancellationToken: cancellationToken);

            return result.ToAppMailMessage();
        }

        public class OutlookMailServiceAuthenticationProvider : IAuthenticationProvider
        {
            public string AccessToken { get; }

            public OutlookMailServiceAuthenticationProvider(string accessToken)
            {
                AccessToken = accessToken;
            }

            public Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
            {
                request.Headers.Add("Authorization", $"Bearer {AccessToken}");

                return Task.CompletedTask;
            }
        }
    }
}
