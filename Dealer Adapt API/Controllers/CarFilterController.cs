using Dealer_Adapt_API.Data;
using Dealer_Adapt_API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dealer_Adapt_API.Controllers;

/// <summary>
/// Anonymouse type for the Car entity model
/// 
/// ••• Summary •••
/// •	GetCar: Retrieves a car by its ID.
/// •	GetAllCars: Retrieves all cars.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class CarFilterController : ControllerBase
{
    /// <summary>
    /// The ApplicationDbContext instance to interact with the database
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Constructor for the CarController class that takes an ApplicationDbContext instance as a parameter
    /// </summary>
    /// <param name="context"></param>
    public CarFilterController(ApplicationDbContext context)
    {
        _context = context;
    }


    /// <summary>
    /// Retrieves a car by its ID from the database and returns it as an IActionResult object
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCar(Guid id)
    {
        var car = await _context.Cars
            .Include(c => c.Pictures) // Include the Pictures navigation property
            .FirstOrDefaultAsync(c => c.Id == id);

        if (car == null)
        {
            return NotFound();
        }

        //car.ListOfPictureUrls = car.Pictures.Select(p => p.Url).ToList();

        // Create a new Car object with the Picture URLs
        var carWithPictures = new
        {
            car.Id,
            car.Make,
            car.Model,
            car.Mileage,
            car.Price,
            car.Color,
            car.VIN,
            car.Description,
            car.Year,
            car.IsNew,
            PictureUrls = car.Pictures?.Select(p => p.Url).ToList() // Get the Picture URLs
        };

        return Ok(carWithPictures);
    }




    /// <summary>
    /// Retrieves all cars from the database and returns them as an IActionResult object
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    [HttpPost("filter")]
    public async Task<IActionResult> FilterCars([FromBody] CarFilterModel filter)
    {
        var allCars = await _context.Cars.ToListAsync();
        var query = allCars.AsQueryable();

        if (filter.Makes != null && filter.Makes.Any())
        {
            query = query.Where(c => filter.Makes.Contains(c.Make));
        }

        if (filter.Models != null && filter.Models.Any())
        {
            query = query.Where(c => filter.Models.Contains(c.Model));
        }

        if (filter.PriceFrom.HasValue)
        {
            query = query.Where(c => c.Price >= filter.PriceFrom.Value);
        }

        if (filter.PriceTo.HasValue)
        {
            query = query.Where(c => c.Price <= filter.PriceTo.Value);
        }

        if (filter.Colors != null && filter.Colors.Any())
        {
            query = query.Where(c => filter.Colors.Contains(c.Color ?? ""));
        }

        if (filter.MileageUpTo.HasValue)
        {
            query = query.Where(c => c.Mileage <= filter.MileageUpTo.Value);
        }

        if (filter.MaintenanceCostFrom.HasValue)
        {
            query = query.Where(c => c.AverageYearlyMaintenanceCost >= filter.MaintenanceCostFrom.Value);
        }

        if (filter.MaintenanceCostTo.HasValue)
        {
            query = query.Where(c => c.AverageYearlyMaintenanceCost <= filter.MaintenanceCostTo.Value);
        }

        if (filter.YearFrom.HasValue)
        {
            query = query.Where(c => c.Year >= filter.YearFrom.Value);
        }

        if (filter.YearTo.HasValue)
        {
            query = query.Where(c => c.Year <= filter.YearTo.Value);
        }

        if (filter.IsNew.HasValue)
        {
            query = query.Where(c => c.IsNew == filter.IsNew.Value);
        }

        if (!string.IsNullOrEmpty(filter.Keyword))
        {
            query = query.Where(c => c.Make.Contains(filter.Keyword) ||
                                     c.Model.Contains(filter.Keyword) ||
                                     (c.Description != null && c.Description.Contains(filter.Keyword)));
        }

        var filteredCars = query.ToList();

        // Calculate total car count
        var totalCarCount = filteredCars.Count;

        // Calculate available filters based on the entire dataset
        var availableFilters = new
        {
            Makes = allCars.GroupBy(c => c.Make)
                           .Select(g => new { Key = g.Key, Count = g.Count() })
                           .ToList(),

            Models = allCars.GroupBy(c => c.Model)
                            .Select(g => new { Key = g.Key, Count = g.Count() })
                            .ToList(),

            Colors = allCars.GroupBy(c => c.Color)
                            .Select(g => new { Key = g.Key, Count = g.Count() })
                            .ToList(),

            PriceRange = new { Min = allCars.Min(c => c.Price), Max = allCars.Max(c => c.Price) },
            MileageRange = new { Min = allCars.Min(c => c.Mileage), Max = allCars.Max(c => c.Mileage) },
            MaintenanceCostRange = new { Min = allCars.Min(c => c.AverageYearlyMaintenanceCost), Max = allCars.Max(c => c.AverageYearlyMaintenanceCost) },
            YearRange = new { Min = allCars.Min(c => c.Year), Max = allCars.Max(c => c.Year) },
            IsNewOptions = new { New = allCars.Any(c => c.IsNew == true), Used = allCars.Any(c => c.IsNew == false) }
        };

        // Update available filters based on the filtered cars
        var updatedAvailableFilters = new
        {
            availableFilters.Makes,
            Models = filteredCars.GroupBy(c => c.Model)
                                 .Select(g => new { Key = g.Key, Count = g.Count() })
                                 .ToList(),
            availableFilters.Colors,
            availableFilters.PriceRange,
            availableFilters.MileageRange,
            availableFilters.MaintenanceCostRange,
            availableFilters.YearRange,
            availableFilters.IsNewOptions
        };

        return Ok(new
        {
            TotalCarCount = totalCarCount,
            AvailableFilters = updatedAvailableFilters,
            Cars = filteredCars
        });
    }



}
