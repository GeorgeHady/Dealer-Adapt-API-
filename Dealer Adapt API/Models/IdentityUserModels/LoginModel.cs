using System.ComponentModel.DataAnnotations;

namespace Dealer_Adapt_API.Models.IdentityUserModels;

/// <summary>
/// Model for logging in a user
/// </summary>
public class LoginModel
{
    // Email is required
    [DataType(DataType.EmailAddress)]
    public required string Email { get; set; }

    // Password is required
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}
