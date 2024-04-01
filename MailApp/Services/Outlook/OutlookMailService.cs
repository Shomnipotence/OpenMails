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

        private IAccount _account;
        private GraphServiceClient _graphServiceClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken">access token from Microsoft Graph auth api</param>
        public OutlookMailService(IAccount account, string accessToken)
        {
            var pca = PublicClientApplicationBuilder
                .Create(AppSecrets.MicrosoftGraphClientId)
                .WithTenantId(AppSecrets.MicrosoftGraphTenantId)
                .Build();

            var authProvider = new OutlookMailServiceAuthenticationProvider(accessToken);

            _account = account;
            _graphServiceClient = new GraphServiceClient(authProvider);
        }

        public string ServiceName => "Outlook";

        public string Name => _account.GetTenantProfiles().FirstOrDefault()?.ClaimsPrincipal?.FindFirst("name")?.Value ?? string.Empty;
        public string Address => _account.Username;

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

            foreach (var rootFolder in rootFolders)
            {
                await foreach (var childFolder in RecursiveGetAllFoldersInFolder(rootFolder, cancellationToken))
                    yield return childFolder;
            }
        }

        public async IAsyncEnumerable<Models.MailFolder> GetAllCommonFoldersAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var folderName in s_wellKnownFolderNames)
            {
                var result = await _graphServiceClient.Me.MailFolders[folderName].GetAsync(parameters => { }, cancellationToken);
                yield return result.ToCommonMailFolder(GetIconFromWellKnownName(folderName));
            }
        }

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
                yield return message.ToCommonMailMessage();
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
                yield return folder.ToCommonMailFolder();

            while (!string.IsNullOrEmpty(response.OdataNextLink))
            {
                response = await _graphServiceClient.Me.MailFolders
                    .WithUrl(response.OdataNextLink)
                    .GetAsync(cancellationToken: cancellationToken);

                foreach (var folder in response.Value)
                    yield return folder.ToCommonMailFolder();
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
                yield return childFolder.ToCommonMailFolder();

            while (!string.IsNullOrEmpty(response.OdataNextLink))
            {
                response = await _graphServiceClient.Me.MailFolders
                    .WithUrl(response.OdataNextLink)
                    .GetAsync(cancellationToken: cancellationToken);

                foreach (var nextChildFolder in response.Value)
                    yield return nextChildFolder.ToCommonMailFolder();
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
            await foreach (var childFolder in GetAllFoldersInFolderAsync(folder, cancellationToken))
            {
                yield return childFolder;

                await foreach (var childFolder2 in RecursiveGetAllFoldersInFolder(childFolder, cancellationToken))
                    yield return childFolder2;
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
                yield return message.ToCommonMailMessage();

            while (!string.IsNullOrEmpty(response.OdataNextLink))
            {
                response = await _graphServiceClient.Me.Messages
                    .WithUrl(response.OdataNextLink)
                    .GetAsync(cancellationToken: cancellationToken);

                foreach (var message in response.Value)
                    yield return message.ToCommonMailMessage();
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
            }, cancellationToken).ConfigureAwait(false);

            foreach (var message in response.Value)
                yield return message.ToCommonMailMessage();
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
                yield return message.ToCommonMailMessage();

            while (!string.IsNullOrEmpty(response.OdataNextLink))
            {
                response = await _graphServiceClient.Me.Messages
                    .WithUrl(response.OdataNextLink)
                    .GetAsync(cancellationToken: cancellationToken);

                foreach (var message in response.Value)
                    yield return message.ToCommonMailMessage();
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
            }, cancellationToken).ConfigureAwait(false);

            foreach (var message in response.Value)
                yield return message.ToCommonMailMessage();
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
