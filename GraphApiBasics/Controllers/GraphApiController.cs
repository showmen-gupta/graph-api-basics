using GraphApiBasics.Interfaces;
using GraphApiBasics.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GraphApiBasics.Controllers;

/// <summary>
///     Graph Api functions
/// </summary>
[Route("api/v1/graph")]
public class GraphApiController(IOptions<GraphSecretOptions> graphSecretOptions, IGraphService graphService)
    : Controller
{
    private readonly GraphSecretOptions _graphSecretOptions = graphSecretOptions.Value;


    [HttpGet("get-access-token-confidential-client-credentials",
        Name = "GetAccessTokenWithConfidentialClientCredential")]
    public async Task<IActionResult> GetAccessTokenWithConfidentialClientCredential()
    {
        var clientId = _graphSecretOptions.ClientId;
        var authority = _graphSecretOptions.Authority;
        var clientSecret = _graphSecretOptions.ClientSecret;
        var tenantId = _graphSecretOptions.TenantId;

        var accessToken =
            await graphService.GetAccessTokenConfidentialClientAsync(clientId, tenantId, clientSecret, authority);

        return Ok(new
            {
                accessToken
            }
        );
    }

    [HttpGet("get-access-token-client-credentials", Name = "GetAccessTokenWithClientCredential")]
    public async Task<IActionResult> GetAccessTokenWithClientCredential()
    {
        var clientId = _graphSecretOptions.ClientId;
        var clientSecret = _graphSecretOptions.ClientSecret;
        var tenantId = _graphSecretOptions.TenantId;

        var accessToken =
            await graphService.GetAccessTokenWithClientCredentialAsync(clientId, tenantId, clientSecret);

        return Ok(new
            {
                accessToken
            }
        );
    }

    [HttpPost("create-user-if-not-exists", Name = "CreateUserIfNotExists")]
    public async Task<IActionResult> CreateUserIfNotExists(string userEmail, string password, string displayName)
    {
        var clientId = _graphSecretOptions.ClientId;
        var clientSecret = _graphSecretOptions.ClientSecret;
        var tenantId = _graphSecretOptions.TenantId;

        var graphClient = await graphService.GetGraphServiceClient(clientId, tenantId, clientSecret);
        var validUser = await graphService.GetUserIfExists(graphClient, userEmail);

        if (validUser != null) return NotFound("User Already Exists");

        var user = await graphService.CreateUserAsync(graphClient, displayName, userEmail, password);
        return Ok(new
        {
            user
        });
    }

    [HttpGet("get-list-of-users", Name = "GetUserList")]
    public async Task<IActionResult> GetUsersList()
    {
        var clientId = _graphSecretOptions.ClientId;
        var clientSecret = _graphSecretOptions.ClientSecret;
        var tenantId = _graphSecretOptions.TenantId;

        var graphClient = await graphService.GetGraphServiceClient(clientId, tenantId, clientSecret);
        var users = await graphService.GetUserListAsync(graphClient)!;


        return Ok(new
        {
            users
        });
    }

    [HttpGet("get-page-iterator", Name = "GetPageIterator")]
    public async Task<IActionResult> GetPageIterator()
    {
        var clientId = _graphSecretOptions.ClientId;
        var clientSecret = _graphSecretOptions.ClientSecret;
        var tenantId = _graphSecretOptions.TenantId;

        var graphClient = await graphService.GetGraphServiceClient(clientId, tenantId, clientSecret);
        var pageIterator = await graphService.GetPageIterator(graphClient)!;
        await pageIterator.IterateAsync();

        return Ok(new
        {
            pageIterator
        });
    }

    [HttpGet("get-users-with-batch-request", Name = "GetUsersWithBatchRequest")]
    public async Task<IActionResult> GetUsersWithBatchRequest()
    {
        var clientId = _graphSecretOptions.ClientId;
        var clientSecret = _graphSecretOptions.ClientSecret;
        var tenantId = _graphSecretOptions.TenantId;

        var graphClient = await graphService.GetGraphServiceClient(clientId, tenantId, clientSecret);
        var users = await graphService.GetUsersWithBatchRequest(graphClient)!;

        return Ok(new
        {
            users
        });
    }

    [HttpGet("get-currently-logged-in-user-info", Name = "GetCurrentlyLoggedInUserInfo")]
    public async Task<IActionResult> GetCurrentlyLoggedInUserInfo()
    {
        var clientId = _graphSecretOptions.ClientId;
        var clientSecret = _graphSecretOptions.ClientSecret;
        var tenantId = _graphSecretOptions.TenantId;

        var graphClient = await graphService.GetGraphServiceClient(clientId, tenantId, clientSecret);
        var loggedInUserInfo = await graphService.GetCurrentlyLoggedInUserInfo(graphClient)!;

        return Ok(new
        {
            loggedInUserInfo
        });
    }

    [HttpGet("get-users-count", Name = "GetUsersCount")]
    public async Task<IActionResult> GetUsersCount()
    {
        var clientId = _graphSecretOptions.ClientId;
        var clientSecret = _graphSecretOptions.ClientSecret;
        var tenantId = _graphSecretOptions.TenantId;

        var graphClient = await graphService.GetGraphServiceClient(clientId, tenantId, clientSecret);
        var usersCount = await graphService.GetUsersCount(graphClient)!;

        return Ok(new
        {
            usersCount
        });
    }

    [HttpGet("get-users-in-group", Name = "GetUsersInGroup")]
    public async Task<IActionResult> GetUsersInGroup()
    {
        var clientId = _graphSecretOptions.ClientId;
        var clientSecret = _graphSecretOptions.ClientSecret;
        var tenantId = _graphSecretOptions.TenantId;

        var graphClient = await graphService.GetGraphServiceClient(clientId, tenantId, clientSecret);
        var usersInGroup = await graphService.GetUsersInGroup(graphClient, "test_id")!;

        return Ok(new
        {
            usersInGroup
        });
    }

    [HttpGet("get-applications-in-group", Name = "GetApplicationsInGroup")]
    public async Task<IActionResult> GetApplicationsInGroup()
    {
        var clientId = _graphSecretOptions.ClientId;
        var clientSecret = _graphSecretOptions.ClientSecret;
        var tenantId = _graphSecretOptions.TenantId;

        var graphClient = await graphService.GetGraphServiceClient(clientId, tenantId, clientSecret);
        var applicationsInGroup = await graphService.GetApplicationsInGroup(graphClient, "test_id")!;

        return Ok(new
        {
            applicationsInGroup
        });
    }

    [HttpPost("get-access-token-username-password", Name = "GetAccessTokenWithUserNamePassword")]
    public async Task<IActionResult> GetAccessTokenWithUserNamePassword(string userName, string password)
    {
        var clientId = _graphSecretOptions.ClientId;
        var authority = _graphSecretOptions.Authority;
        var scopes = new[]
        {
            "User.Read, User.ReadAll"
        };

        var accessToken =
            await graphService.GetAccessTokenByUserNamePassword(clientId, scopes, authority, userName, password);

        return Ok(new
            {
                accessToken
            }
        );
    }
}