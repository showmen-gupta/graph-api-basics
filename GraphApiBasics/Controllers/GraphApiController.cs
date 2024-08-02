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


    [HttpGet("get-access-token-confidential-client-credentials", Name = "GetAccessTokenWithConfidentialClientCredential")]
    public async Task<IActionResult> GetAccessTokenWithClientCredential()
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
}