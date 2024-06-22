using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityNet8.Database;

public class User : IdentityUser
{
    public string? Initials { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}