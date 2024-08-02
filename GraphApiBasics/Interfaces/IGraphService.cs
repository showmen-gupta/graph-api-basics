using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace GraphApiBasics.Interfaces;

//install Microsoft.graph
public interface IGraphService
{
    /// <summary>
    ///     Azure Ad login either silently or interactvely
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
    ///     Azure Ad login either silently or with client crendential
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
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="scopes"></param>
    /// <param name="authority"></param>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public Task<string> GetAccessTokenByUserNamePassword(string clientId, ICollection<string> scopes, string authority,
        string userName, string password);

    /// <summary>
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="tenantId"></param>
    /// <param name="clientSecret"></param>
    /// <returns></returns>
    public Task<GraphServiceClient> GetGraphServiceClient(string clientId, string tenantId, string clientSecret);

    /// <summary>
    /// </summary>
    /// <param name="graphClient"></param>
    /// <param name="userEmail"></param>
    /// <returns></returns>
    public Task<User?> GetUserIfExists(GraphServiceClient graphClient, string userEmail);

    /// <summary>
    /// </summary>
    /// <param name="graphClient"></param>
    /// <param name="displayName"></param>
    /// <param name="userPrincipalName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public Task<User?> CreateUserAsync(GraphServiceClient graphClient, string? displayName, string userPrincipalName,
        string password);
}