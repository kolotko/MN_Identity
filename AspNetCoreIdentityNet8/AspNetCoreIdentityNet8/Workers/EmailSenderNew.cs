using AspNetCoreIdentityNet8.Database;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityNet8.Workers;

public class EmailSenderNew : IEmailSender<User>
{
    public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
    {
        await Task.Delay(1000);
    }

    public async Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
    {
        await Task.Delay(1000);
    }

    public async Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {
        await Task.Delay(1000);
    }
}