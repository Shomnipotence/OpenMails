using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using OpenMails.Extensions;
using OpenMails.Models;
using Windows.UI.Xaml.Media;

#nullable enable

namespace OpenMails.Services.Outlook
{
    public class OutlookMailService : IMailService
    {
        static readonly string[] s_graphScoped = new[] { "Mail.ReadWrite" };

        IAccount _account;
        GraphServiceClient _graphServiceClient;

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
        public ImageSource Avatar
        {
            get
            {
                throw new NotImplementedException();
            }
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
            }, cancellationToken);

            foreach (var message in response.Value)
                yield return message.ToCommonMailMessage();
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
