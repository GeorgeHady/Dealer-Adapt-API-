using System.ComponentModel.DataAnnotations;

namespace Dealer_Adapt_API.Models.IdentityUserModels;

/// <summary>
/// Model for resetting a user's password
/// </summary>
public class ResetPasswordModel
{
    // Email address of the user
    [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
    [DataType(DataType.EmailAddress)]
    public required string Email { get; set; }

    // Token for resetting the password

    public required string Token { get; set; }

    // New password for the user
    [Display(Name = "New Password")]
    [DataType(DataType.Password)]
    public required string NewPassword { get; set; }
}

