using System.ComponentModel.DataAnnotations;

namespace Dealer_Adapt_API.Models.IdentityUserModels;


public class RefreshTokenModel
{
    [Required]
    public string Email { get; set; }


    [Required]
    public string RefreshToken { get; set; }
}
