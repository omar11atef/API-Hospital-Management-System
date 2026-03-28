namespace Hospital_Management_System.Services;

public class EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger) : IEmailSender
{
    private readonly MailSettings _mailSettings = mailSettings.Value;
    private readonly ILogger<EmailService> _logger = logger;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Message come where , type subject :
        var Message = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_mailSettings.Mail),
            Subject = subject,
        };
        // Whose Email that i sender here this message :
        Message.To.Add(MailboxAddress.Parse(email));
        // Generation Email Body :
        var builder = new BodyBuilder
        {
            HtmlBody = htmlMessage
        };
        Message.Body = builder.ToMessageBody();
        using var smtp = new SmtpClient();
        _logger.LogInformation("Sending Email To {email}", email);

        smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
        await smtp.SendAsync(Message);

        smtp.Disconnect(true);

    }
}
