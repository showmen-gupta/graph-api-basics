using Azure.Core;
using Azure.Identity;
using GraphApiBasics.Interfaces;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;

namespace GraphApiBasics.Services;

/// <summary>
///     A service for Microsoft Graph Api
/// </summary>
/// <remarks>
/// </remarks>
public class GraphService(ILoggerFactory loggerFactory) : IGraphService
{
    /// <summary>
    /// </summary>
    /// <param name="loggerFactory"></param>
    private readonly ILogger<GraphService> _logger = loggerFactory.CreateLogger<GraphService>();

    /// <inheritdoc />
    public async Task<string> GetAccessTokenConfidentialClientAsync(string clientId, string tenantId,
        string clientSecret, string authority)
    {
        // Define the scopes you need
        var scopes = new[]
        {
            "https://graph.microsoft.com/.default"
        };

        try
        {
            var publicClient = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(authority)
                .WithTenantId(tenantId)
                .WithRedirectUri("http://localhost:7181/auth/login-callback-ms")
                .Build();

            var token = await publicClient.AcquireTokenForClient(scopes)
                .WithTenantIdFromAuthority(new Uri(authority))
                .ExecuteAsync();

            var accessToken = token.AccessToken;

            return accessToken;
        }
        catch (MsalUiRequiredException ex)
        {
            // The user needs to sign in interactively
            _logger.LogCritical($"Error acquiring token: {ex.Message}");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<string> GetAccessTokenWithClientCredentialAsync(string clientId, string tenantId,
        string clientSecret,
        CancellationToken cancellationToken = default)
    {
        // Define the scopes you need
        var scopes = new[]
        {
            "https://graph.microsoft.com/.default"
        };

        try
        {
            var options = new ClientSecretCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret, options);

            var tokenRequestContext = new TokenRequestContext(scopes);
            var token = await credential.GetTokenAsync(tokenRequestContext, cancellationToken);
            var accessToken = token.Token;

            return accessToken;
        }
        catch (MsalUiRequiredException ex)
        {
            // The user needs to sign in interactively
            _logger.LogCritical($"Error acquiring token: {ex.Message}");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<string> GetAccessTokenByUserNamePassword(string clientId, ICollection<string> scopes,
        string authority, string userName,
        string password)
    {
        try
        {
            var app = PublicClientApplicationBuilder.Create(clientId)
                .WithAuthority(authority)
                .WithRedirectUri("http://localhost:7181/auth/login-callback-ms")
                .Build();

            var result = await app.AcquireTokenByUsernamePassword(scopes, userName, password)
                .ExecuteAsync();

            return result.AccessToken;
        }
        catch (Exception ex)
        {
            throw new BadHttpRequestException(ex.Message);
        }
    }

    /// <inheritdoc />
    public Task<GraphServiceClient> GetGraphServiceClient(string clientId, string tenantId, string clientSecret)
    {
        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

        var graphClient = new GraphServiceClient(credential);

        return Task.FromResult(graphClient);
    }

    /// <inheritdoc />
    public async Task<User?> GetUserIfExists(GraphServiceClient graphClient, string userEmail)
    {
        try
        {
            var userCollection = await graphClient.Users
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Filter = $"userPrincipalName eq '{userEmail}'";
                });

            var users = userCollection?.Value ?? throw new Exception("No users found");
            return users.FirstOrDefault();
        }
        catch (Exception ex)
        {
            throw new BadHttpRequestException(ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<User?> CreateUserAsync(GraphServiceClient graphClient, string? displayName,
        string userPrincipalName, string password)
    {
        try
        {
            var newUser = new User
            {
                AccountEnabled = true,
                DisplayName = displayName,
                MailNickname = userPrincipalName.Split('@')[0],
                Mail = userPrincipalName,
                UserPrincipalName = userPrincipalName,
                PasswordProfile = new PasswordProfile
                {
                    ForceChangePasswordNextSignIn = true,
                    Password = password
                }
            };

            return await graphClient.Users.PostAsync(newUser);
        }
        catch (Exception ex)
        {
            throw new BadHttpRequestException(ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<List<User>>? GetUserListAsync(GraphServiceClient graphClient)
    {
        try
        {
            var usersResponse = await graphClient
                .Users
                .GetAsync(requestConfiguration =>
                    requestConfiguration.QueryParameters.Select = ["id", "createdDateTime", "userPrincipalName"]);

            var userList = usersResponse?.Value;
            return userList ?? throw new InvalidOperationException();
        }
        catch (Exception ex)
        {
            throw new BadHttpRequestException(ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<PageIterator<User, UserCollectionResponse>>? GetPageIterator(GraphServiceClient graphClient)
    {
        try
        {
            var usersResponse = await graphClient
                .Users
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = new[] { "id", "createdDateTime" };
                    requestConfiguration.QueryParameters.Top = 1;
                });

            var pageIterator = PageIterator<User, UserCollectionResponse>.CreatePageIterator(graphClient,
                usersResponse ?? throw new InvalidOperationException(),
                user =>
                {
                    new List<User>().Add(user);
                    return true;
                });

            return pageIterator;
        }
        catch (Exception ex)
        {
            throw new BadHttpRequestException(ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<List<User>>? GetUsersWithBatchRequest(GraphServiceClient graphClient)
    {
        try
        {
            var requestInformation = graphClient
                .Users
                .ToGetRequestInformation();

            // create the content
            var batchRequestContent = new BatchRequestContentCollection(graphClient);
            // add steps
            var requestStepId = await batchRequestContent.AddBatchRequestStepAsync(requestInformation);

            // send and get back response
            var batchResponseContent = await graphClient.Batch.PostAsync(batchRequestContent);

            var usersResponse = await batchResponseContent.GetResponseByIdAsync<UserCollectionResponse>(requestStepId);
            var userList = usersResponse.Value;

            return userList ?? throw new InvalidOperationException();
        }
        catch (Exception ex)
        {
            throw new BadHttpRequestException(ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<User> GetCurrentlyLoggedInUserInfo(GraphServiceClient graphClient)
    {
        try
        {
            var user = await graphClient
                .Me
                .GetAsync();
            return user ?? throw new InvalidOperationException();
        }
        catch (Exception ex)
        {
            throw new BadHttpRequestException(ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<int?> GetUsersCount(GraphServiceClient graphClient)
    {
        try
        {
            var count = await graphClient.Users.Count.GetAsync(requestConfiguration =>
                requestConfiguration.Headers.Add("ConsistencyLevel", "eventual"));
            return count;
        }
        catch (Exception ex)
        {
            throw new BadHttpRequestException(ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<UserCollectionResponse> GetUsersInGroup(GraphServiceClient graphClient, string groupId)
    {
        try
        {
            var usersInGroup = await graphClient.Groups["group-id"].Members.GraphUser.GetAsync();
            return usersInGroup ?? throw new InvalidOperationException();
        }
        catch (Exception ex)
        {
            throw new BadHttpRequestException(ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<ApplicationCollectionResponse> GetApplicationsInGroup(GraphServiceClient graphClient,
        string groupId)
    {
        try
        {
            var applicationsInGroup = await graphClient.Groups[groupId].Members.GraphApplication.GetAsync();
            return applicationsInGroup ?? throw new InvalidOperationException();
        }
        catch (Exception ex)
        {
            throw new BadHttpRequestException(ex.Message);
        }
    }
}