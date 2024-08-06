using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace GraphApiBasics.Interfaces;

//install Microsoft.graph
public interface IGraphService
{
    /// <summary>
    ///     Azure Ad login with ConfidentialClientBuilder
    ///     <param name="clientId"></param>
    ///     <param name="tenantId"></param>
    ///     <param name="clientSecret"></param>
    ///     <param name="authority"></param>
    /// </summary>
    /// <returns>
    ///     access token
    /// </returns>
    public Task<string> GetAccessTokenConfidentialClientAsync(string clientId, string tenantId, string clientSecret,
        string authority);

    /// <summary>
    ///     Azure Ad login with client crendential
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="tenantId"></param>
    /// <param name="clientSecret"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     access token
    /// </returns>
    public Task<string> GetAccessTokenWithClientCredentialAsync(string clientId, string tenantId, string clientSecret,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Azure Ad login with user name and password
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="scopes"></param>
    /// <param name="authority"></param>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <returns>AccessToken</returns>
    public Task<string> GetAccessTokenByUserNamePassword(string clientId, ICollection<string> scopes, string authority,
        string userName, string password);

    /// <summary>
    ///     Getting graph service client
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="tenantId"></param>
    /// <param name="clientSecret"></param>
    /// <returns>GraphClient</returns>
    public Task<GraphServiceClient> GetGraphServiceClient(string clientId, string tenantId, string clientSecret);

    /// <summary>
    ///     See if the user exists with user email
    /// </summary>
    /// <param name="graphClient"></param>
    /// <param name="userEmail"></param>
    /// <returns>Existing user</returns>
    public Task<User?> GetUserIfExists(GraphServiceClient graphClient, string userEmail);

    /// <summary>
    ///     Create a new user with graph api
    /// </summary>
    /// <param name="graphClient"></param>
    /// <param name="displayName"></param>
    /// <param name="userPrincipalName"></param>
    /// <param name="password"></param>
    /// <returns>Created User</returns>
    public Task<User?> CreateUserAsync(GraphServiceClient graphClient, string? displayName, string userPrincipalName,
        string password);

    /// <summary>
    ///     Getting list of all users on the tenant
    /// </summary>
    /// <param name="graphClient"></param>
    /// <returns>All Users as list for a specific tenant</returns>
    public Task<List<User>>? GetUserListAsync(GraphServiceClient graphClient);

    /// <summary>
    ///     PageIterator to automatically page through result sets across multiple calls and process each item in
    ///     the result set.
    /// </summary>
    /// <param name="graphClient"></param>
    /// <returns>
    ///     PageIterator
    /// </returns>
    public Task<PageIterator<User, UserCollectionResponse>>? GetPageIterator(GraphServiceClient graphClient);

    /// <summary>
    ///     Apart from passing instances of HttpRequestMessage, batch requests support the passing of RequestInformation
    ///     instances as follows.
    /// </summary>
    /// <param name="graphClient"></param>
    /// <returns>All Users as list with batch request</returns>
    public Task<List<User>>? GetUsersWithBatchRequest(GraphServiceClient graphClient);

    /// <summary>
    ///     Getting info for the currently authenticated user
    /// </summary>
    /// <param name="graphClient"></param>
    /// <returns>Currently logged in user info</returns>
    public Task<User> GetCurrentlyLoggedInUserInfo(GraphServiceClient graphClient);

    /// <summary>
    ///     Get count of users in a tenant
    /// </summary>
    /// <param name="graphClient"></param>
    /// <returns>Get User counts with from graph api</returns>
    public Task<int?> GetUsersCount(GraphServiceClient graphClient);

    /// <summary>
    ///     Get users of a specific group that those users belong
    /// </summary>
    /// <param name="graphClient"></param>
    /// <param name="groupId"></param>
    /// <returns> Users in group</returns>
    public Task<UserCollectionResponse> GetUsersInGroup(GraphServiceClient graphClient, string groupId);

    /// <summary>
    ///     Get applications of a specific group that belongs to that tenant
    /// </summary>
    /// <param name="graphClient"></param>
    /// <param name="groupId"></param>
    /// <returns> Applications in group</returns>
    public Task<ApplicationCollectionResponse> GetApplicationsInGroup(GraphServiceClient graphClient, string groupId);
}