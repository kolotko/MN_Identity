using JwtAuthorization.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthorization.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonController : ControllerBase
{
    private List<string> _personList;
    public PersonController()
    {
        _personList = new();
        _personList.Add("Joe");
        _personList.Add("Donald");
        _personList.Add("Melania");
    }
    
    [HttpGet("GetPersonListNoAuthentication")]
    public List<string> GetPersonListNoAuthentication()
    {
        return _personList;
    }
    
    [Authorize]
    [HttpGet("GetPersonListAuthentication")]
    public List<string> GetPersonListAuthentication()
    {
        return _personList;
    }
    
    [Authorize(Policy = IdentityData.AdminUserClaimPolicy)]
    [HttpGet("GetPersonListAuthenticationAdmin")]
    public List<string> GetPersonListAuthenticationAdmin()
    {
        return _personList;
    }
    
    [Authorize]
    [RequiresClaim(IdentityData.AdminUserClaimName, "True")]
    [HttpGet("GetPersonListAuthenticationAdminFilter")]
    public List<string> GetPersonListAuthenticationAdminFilter()
    {
        return _personList;
    }
}