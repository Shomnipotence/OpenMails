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
using OpenMails;
using OpenMails.Extensions;
using OpenMails.Models;
using Windows.UI.Xaml.Media;

#nullable enable

namespace OpenMails.Services
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


        public async IAsyncEnumerable<Models.MailFolder> GetAllFoldersAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var response = await _graphServiceClient.Me.MailFolders.GetAsync(parameters => { }, cancellationToken);

            foreach (var folder in response.Value)
                yield return new OpenMails.Models.MailFolder(folder.Id, folder.DisplayName, folder.DisplayName, folder.ParentFolderId);
        }

        public async IAsyncEnumerable<MailMessage> GetAllMessagesAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var response = await _graphServiceClient.Me.Messages.GetAsync(parameter => { }, cancellationToken);

            foreach (var message in response.Value)
                yield return message.ToCommonMailMessage();
        }

        public async IAsyncEnumerable<MailMessage> GetAllMessagesInFolder(
            Models.MailFolder folder, 
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var response = await _graphServiceClient.Me.MailFolders[folder.Id].Messages.GetAsync(parameters => { }, cancellationToken);

            foreach (var message in response.Value)
                yield return message.ToCommonMailMessage();
        }

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
