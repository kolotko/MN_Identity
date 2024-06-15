using AspNetCoreIdentityNet8.Database;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentityNet8.Extensions;

public static class MiggrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        context.Database.Migrate();
    }
}