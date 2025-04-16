using System.ComponentModel.DataAnnotations;

namespace Dealer_Adapt_API.Models.IdentityUserModels;

/// <summary>
/// Model for the forgot password endpoint.
/// </summary>
public class ForgotPasswordModel
{
    // Email address of the user who forgot their password.
    [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
    public required string Email { get; set; }
}
