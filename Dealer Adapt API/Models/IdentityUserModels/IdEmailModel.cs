namespace Dealer_Adapt_API.Models.IdentityUserModels;

/// <summary>
/// Model for returning the Id or/and Email of a user will be used in the response of the API returning in body
/// </summary>
public class IdEmailModel
{
    public string? Id { get; set; }
    public string? Email { get; set; }
}
