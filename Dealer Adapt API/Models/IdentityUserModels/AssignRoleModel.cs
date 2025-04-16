using System.ComponentModel.DataAnnotations;

namespace Dealer_Adapt_API.Models.IdentityUserModels;

/// <summary>
/// Model for assigning a role to a user.
/// </summary>
public class AssignRoleModel
{
    // The user's email address to assign the role to.
    [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
    public required string Email { get; set; }

    // The role to assign to the user.
    [StringLength(50, ErrorMessage = "Role cannot be longer than 50 characters.")]
    public required string Role { get; set; }
}
