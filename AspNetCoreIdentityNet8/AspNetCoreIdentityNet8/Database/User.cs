using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityNet8.Database;

public class User : IdentityUser
{
    public string? Initials { get; set; }
}