using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Dealer_Adapt_API.Models.Entities;

/// <summary>
/// Model for picture of cars
/// </summary>
public class Picture
{
    // holds the id of the picture
    [Key]
    public Guid Id { get; set; }

    // holds the url of the picture
    [Url(ErrorMessage = "Url must be a valid URL.")]
    public required string Url { get; set; }

    // holds foreign key of the car id that the picture belongs to
    public Guid? CarId { get; set; }


    // navigation property for the Car entity
    [JsonIgnore]
    [ForeignKey("CarId")]
    public Car? Car { get; set; }


}
