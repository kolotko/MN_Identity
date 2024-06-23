using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityNet8.Database;

public class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        

        string[] roleNames = { "Admin", "User", "Manager" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));

                
                if (roleName == "Admin")
                {
                    var role = await roleManager.FindByNameAsync(roleName);
                    var claim = new Claim("CustomClaimTypeRole", "CustomClaimValueRole");
                    var result = await roleManager.AddClaimAsync(role, claim);
                }
            }
        }

        var powerUser = new User
        {
            UserName = "admin@example.com",
            Email = "admin@example.com",
        };

        string userPWD = "Password123!";
        var _user = await userManager.FindByEmailAsync("admin@example.com");

        if (_user == null)
        {
            var createPowerUser = await userManager.CreateAsync(powerUser, userPWD);
            if (createPowerUser.Succeeded)
            {
                await userManager.AddToRoleAsync(powerUser, "Admin");
            }
        }
        
        var claims = await userManager.GetClaimsAsync(_user);
        bool hasClaim = claims.Any(c => c.Type == "CustomClaimType" && c.Value == "CustomClaimValue");

        if (!hasClaim)
        {
            var claim = new Claim("CustomClaimType", "CustomClaimValue");
            var result = await userManager.AddClaimAsync(_user, claim);
        }
    }
}