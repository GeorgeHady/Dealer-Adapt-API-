namespace Dealer_Adapt_API.Models.Entities;

/// <summary>
/// Model for the car filter model that is used to filter cars
/// </summary>
public class CarFilterModel
{
    public List<string>? Makes { get; set; }
    public List<string>? Models { get; set; }
    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }
    public List<string>? Colors { get; set; }
    public int? MileageUpTo { get; set; }
    public decimal? MaintenanceCostFrom { get; set; }
    public decimal? MaintenanceCostTo { get; set; }
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
    public bool? IsNew { get; set; }
    public string? Keyword { get; set; }
}
