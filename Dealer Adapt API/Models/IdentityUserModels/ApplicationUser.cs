using Dealer_Adapt_API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Dealer_Adapt_API.Models.IdentityUserModels;


/// <summary>
/// ApplicationUser class that inherits from IdentityUser
/// This class is used to store the user information
/// </summary>
public class ApplicationUser : IdentityUser
{
    // First Name is required
    [StringLength(50, ErrorMessage = "First Name cannot be longer than 100 characters.")]
    public required string FirstName { get; set; }

    // Last Name is optional
    [StringLength(50, ErrorMessage = "Last Name cannot be longer than 100 characters.")]
    public string? LastName { get; set; }

    // Birth Date is optional
    [DataType(DataType.Date)]
    [Range(typeof(DateOnly), "1/1/1900", "12/31/2999", ErrorMessage = "Invalid Date")]
    public DateOnly? DoB { get; set; }


    //// Navigation property for the Favorite entity
    public ICollection<Favorite>? Favorites { get; set; }

}
