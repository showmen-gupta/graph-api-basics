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

        if (validUser != null) return BadRequest("User Already Exists");

        var user = await graphService.CreateUserAsync(graphClient, displayName, userEmail, password);
        return Ok(new
        {
            user
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