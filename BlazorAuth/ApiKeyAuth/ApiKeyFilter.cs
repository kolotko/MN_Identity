using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiKeyAuth;

public class ApiKeyFilter : IAuthorizationFilter
{
    private readonly IConfiguration _configuration;

    public ApiKeyFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("x-api-key", out var extractedApikey))
        {
            context.Result = new UnauthorizedObjectResult("Api Key missing"); return; 
        } 

        var apiKey = _configuration["ApiKey"]!;
        if (apiKey != extractedApikey)
        {
            context.Result = new UnauthorizedObjectResult("Api Key missing"); 
            return;
        }
    }
}