using Microsoft.AspNetCore.Identity.UI.Services;

namespace AspNetCoreIdentityNet8.Workers;

public class EmailSenderOld : IEmailSender
{

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        await Task.Delay(1000);
    }
}