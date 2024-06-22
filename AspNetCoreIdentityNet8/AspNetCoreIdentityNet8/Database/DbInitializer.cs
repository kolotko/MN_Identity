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
    }
}