using Dealer_Adapt_API.Data;
using Dealer_Adapt_API.Models.IdentityUserModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Dealer_Adapt_API.Models.Entities;

/// <summary>
/// Model for favorites
/// </summary>
public class Favorite
{
    [Key]
    public Guid Id { get; set; }

    // Foreign keys for the car and the user
    //[ForeignKey("Car")]
    public required Guid CarId { get; set; }

    // Foreign key for the user that made the favorite
    //[ForeignKey("User")]
    public required string UserId { get; set; }

    // DateTime holds the DateTime.Now value when the favorite is created
    // This is used to sort the favorites by the most recent
    [DataType(DataType.DateTime)]
    public DateTime DateTime { get; set; }


    //// Navigation properties

    // Navigation properties
    [JsonIgnore]
    [ForeignKey("CarId")]
    public Car Car { get; set; }

    [JsonIgnore]
    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }
}
