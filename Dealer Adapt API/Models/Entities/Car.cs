using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Dealer_Adapt_API.Models.Entities;

/// <summary>
/// Represents a car entity model to interact with the database
/// </summary>
public class Car
{
    // the unique identifier of the car
    [Key]
    public Guid Id { get; set; }

    // make of the car
    [StringLength(50, ErrorMessage = "Make cannot be longer than 50 characters.")]
    public required string Make { get; set; }

    // model of the car
    [StringLength(50, ErrorMessage = "Model cannot be longer than 50 characters.")]
    public required string Model { get; set; }

    // the mileage of the car
    [Range(0, int.MaxValue, ErrorMessage = "Mileage must be a positive number.")]
    public int? Mileage { get; set; }

    // price of the car
    //[Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
    //[Column(TypeName = "decimal(18,2)")]
    [Range(0, 1000000, ErrorMessage = "Price must be a positive number.")]
    [DataType(DataType.Currency)]
    public decimal? Price { get; set; }

    // color of the car
    [StringLength(20, ErrorMessage = "Color cannot be longer than 20 characters.")]
    public string? Color { get; set; }

    // the Vehicle Identification Number of the car
    [StringLength(17, ErrorMessage = "VIN cannot be longer than 17 characters.")]
    public string? VIN { get; set; }

    // the average cost of the car's annual maintenance'
    // it is nullable because it is not required
    // this value is in dollars
    // this value should get it from external API resources like Edmunds or KBB or NADA etc.
    [Range(0, double.MaxValue, ErrorMessage = "Average Yearly Maintenance Cost must be a positive number.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal? AverageYearlyMaintenanceCost { get; set; }

    // description of the car
    [StringLength(5000, ErrorMessage = "Description cannot be longer than 5000 characters.")]
    public string? Description { get; set; }

    // holds the year of the car
    // In July 1886 the newspapers reported on the first public outing of the three-wheeled Benz Patent Motor Car, model no. 1.
    [Range(1886, 2500, ErrorMessage = "Year must be a valid year.")]
    public int? Year { get; set; }

    // holds the status of the car used or new
    public bool? IsNew { get; set; }


    // Foreign key for the cover image to Store a Picture ID
    public Guid? CoverImageId { get; set; }



    /// ***** Navigation properties ***** ///

    // Navigation property for the Picture entity
    //[JsonIgnore]
    [InverseProperty("Car")]
    public ICollection<Picture>? Pictures { get; set; }

    //Navigation property for the cover image
    [JsonIgnore]
    [ForeignKey("CoverImageId")]
    public Picture? Picture { get; set; }

    //// Navigation property for the Favorite entity
    [JsonIgnore]
    public ICollection<Favorite>? Favorites { get; set; }


    // NotMapped attribute is used to exclude a property from the database
    // this property to handle picture URLs if needed
    [NotMapped]
    public List<string>? ListOfPictureUrls { get; set; }
}
