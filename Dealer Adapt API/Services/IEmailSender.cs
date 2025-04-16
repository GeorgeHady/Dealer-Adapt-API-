namespace Dealer_Adapt_API.Servicesl;

/// <summary>
/// Interface for sending emails to users for account confirmation, password reset, etc.
/// </summary>
public interface IEmailSender
{
    Task SendEmailAsync(string email
        , string subject
        , string message
        //, string? SenderEmailShown = null
        , string? SenderName = null);
}
