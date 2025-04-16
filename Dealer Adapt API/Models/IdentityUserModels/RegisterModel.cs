using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dealer_Adapt_API.Models.IdentityUserModels;

/// <summary>
/// Model for registering a new user
/// </summary>
public class RegisterModel
{
    // Email address of the user
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
    public required string Email { get; set; }

    // Password of the user
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    // Confirm password of the user
    [DataType(DataType.Password)]
    //[Compare(nameof(Password))]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    [DisplayName("Confirm Password")]
    public required string ConfirmPassword { get; set; }

    // First name of the user
    [StringLength(50, ErrorMessage = "First Name cannot be longer than 100 characters.")]
    public required string FirstName { get; set; }

    // Last name of the user
    [StringLength(50, ErrorMessage = "Last Name cannot be longer than 100 characters.")]
    public string? LastName { get; set; }

    // Phone number of the user
    [Phone(ErrorMessage = "Phone number must be a valid phone number.")]
    public string? PhoneNumber { get; set; }


    // Birth date of the user
    [DataType(DataType.Date)]
    [Range(typeof(DateOnly), "1/1/1900", "12/31/2999", ErrorMessage = "Invalid Date")]
    public DateOnly? DoB { get; set; }

}
