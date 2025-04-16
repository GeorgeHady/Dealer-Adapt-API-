using Dealer_Adapt_API.Servicesl;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Dealer_Adapt_API.Services;

/// <summary>
/// Service for sending emails to users for account confirmation, password reset, etc.
/// </summary>
public class EmailSender : IEmailSender
{
    
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Constructor for EmailSender
    /// </summary>
    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    /// <summary>
    /// Sends an email to the specified email address with the specified subject and message.
    /// </summary>
    public async Task SendEmailAsync(string email, string subject, string message, string? SenderName = null)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");
        var smtpClient = new SmtpClient(emailSettings["SmtpServer"])
        {
            Port = int.Parse(emailSettings["SmtpPort"]),
            Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(emailSettings["SenderEmail"], SenderName ?? emailSettings["SenderName"]),
            Subject = subject,
            Body = message,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(email);

        await smtpClient.SendMailAsync(mailMessage);
    }
}
