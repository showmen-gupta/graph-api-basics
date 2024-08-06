using GraphApiBasics.Interfaces;
using GraphApiBasics.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Graph;

namespace GraphApiBasics.Controllers;

/// <summary>
/// Graph API functions
/// </summary>
[Route("api/v1/graph")]
public class GraphApiController(IOptions<GraphSecretOptions> graphSecretOptions, IGraphService graphService)
    : Controller
{
    private readonly GraphSecretOptions _graphSecretOptions = graphSecretOptions.Value;
    private static readonly string[] Scopes = new[] { "User.Read", "User.ReadAll" };

    private async Task<GraphServiceClient> GetGraphClientAsync()
    {
        return await graphService.GetGraphServiceClient(_graphSecretOptions.ClientId, _graphSecretOptions.TenantId, _graphSecretOptions.ClientSecret);
    }

    [HttpGet("get-access-token-confidential-client-credentials", Name = "GetAccessTokenWithConfidentialClientCredential")]
    public async Task<IActionResult> GetAccessTokenWithConfidentialClientCredential()
    {
        var accessToken = await graphService.GetAccessTokenConfidentialClientAsync(
            _graphSecretOptions.ClientId,
            _graphSecretOptions.TenantId,
            _graphSecretOptions.ClientSecret,
            _graphSecretOptions.Authority
        );

        return Ok(new { accessToken });
    }

    [HttpGet("get-access-token-client-credentials", Name = "GetAccessTokenWithClientCredential")]
    public async Task<IActionResult> GetAccessTokenWithClientCredential()
    {
        var accessToken = await graphService.GetAccessTokenWithClientCredentialAsync(
            _graphSecretOptions.ClientId,
            _graphSecretOptions.TenantId,
            _graphSecretOptions.ClientSecret
        );

        return Ok(new { accessToken });
    }

    [HttpPost("create-user-if-not-exists", Name = "CreateUserIfNotExists")]
    public async Task<IActionResult> CreateUserIfNotExists(string userEmail, string password, string displayName)
    {
        var graphClient = await GetGraphClientAsync();
        var validUser = await graphService.GetUserIfExists(graphClient, userEmail);

        if (validUser != null)
        {
            return NotFound("User Already Exists");
        }

        var user = await graphService.CreateUserAsync(graphClient, displayName, userEmail, password);
        return Ok(new { user });
    }

    [HttpGet("get-list-of-users", Name = "GetUserList")]
    public async Task<IActionResult> GetUsersList()
    {
        var graphClient = await GetGraphClientAsync();
        var users = await graphService.GetUserListAsync(graphClient)!;
        return Ok(new { users });
    }

    [HttpGet("get-page-iterator", Name = "GetPageIterator")]
    public async Task<IActionResult> GetPageIterator()
    {
        var graphClient = await GetGraphClientAsync();
        var pageIterator = await graphService.GetPageIterator(graphClient)!;
        await pageIterator.IterateAsync();

        return Ok(new { pageIterator });
    }

    [HttpGet("get-users-with-batch-request", Name = "GetUsersWithBatchRequest")]
    public async Task<IActionResult> GetUsersWithBatchRequest()
    {
        var graphClient = await GetGraphClientAsync();
        var users = await graphService.GetUsersWithBatchRequest(graphClient)!;
        return Ok(new { users });
    }

    [HttpGet("get-currently-logged-in-user-info", Name = "GetCurrentlyLoggedInUserInfo")]
    public async Task<IActionResult> GetCurrentlyLoggedInUserInfo()
    {
        var graphClient = await GetGraphClientAsync();
        var loggedInUserInfo = await graphService.GetCurrentlyLoggedInUserInfo(graphClient);
        return Ok(new { loggedInUserInfo });
    }

    [HttpGet("get-users-count", Name = "GetUsersCount")]
    public async Task<IActionResult> GetUsersCount()
    {
        var graphClient = await GetGraphClientAsync();
        var usersCount = await graphService.GetUsersCount(graphClient);
        return Ok(new { usersCount });
    }

    [HttpGet("get-users-in-group", Name = "GetUsersInGroup")]
    public async Task<IActionResult> GetUsersInGroup()
    {
        var graphClient = await GetGraphClientAsync();
        var usersInGroup = await graphService.GetUsersInGroup(graphClient, "test_id");
        return Ok(new { usersInGroup });
    }

    [HttpGet("get-applications-in-group", Name = "GetApplicationsInGroup")]
    public async Task<IActionResult> GetApplicationsInGroup()
    {
        var graphClient = await GetGraphClientAsync();
        var applicationsInGroup = await graphService.GetApplicationsInGroup(graphClient, "test_id");
        return Ok(new { applicationsInGroup });
    }

    [HttpPost("get-access-token-username-password", Name = "GetAccessTokenWithUserNamePassword")]
    public async Task<IActionResult> GetAccessTokenWithUserNamePassword(string userName, string password)
    {
        var accessToken = await graphService.GetAccessTokenByUserNamePassword(
            _graphSecretOptions.ClientId,
            Scopes,
            _graphSecretOptions.Authority,
            userName,
            password
        );

        return Ok(new { accessToken });
    }
}
