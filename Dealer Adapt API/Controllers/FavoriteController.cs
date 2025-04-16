using Dealer_Adapt_API.Data;
using Dealer_Adapt_API.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dealer_Adapt_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FavoriteController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FavoriteController(ApplicationDbContext context)
    {
        _context = context;
    }


    /// <summary>
    /// Retrieves a list of favorites for a given user ID
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("{userId}")]
    public async Task<IActionResult> FavoritesList(string userId)
    {
        var userFavorites = await _context.Favorites
            .Include(f => f.Car) // Include the Car navigation property
            .Where(f => f.UserId == userId)
            .ToListAsync();

        if (userFavorites == null || !userFavorites.Any())
        {
            return NotFound();
        }

        var coverImageURL = await _context.Pictures
            .FirstOrDefaultAsync(p => p.Id == userFavorites
                .First().Car.CoverImageId);

        var favoritesWithDetails = userFavorites.Select(favorite => new
        {
            favorite.Id,
            favorite.UserId,
            favorite.CarId,
            favorite.DateTime,
            Car = new
            {
                favorite.Car.Id,
                favorite.Car.Make,
                favorite.Car.Model,
                favorite.Car.Mileage,
                favorite.Car.Price,
                favorite.Car.Year
            }
        }).ToList();

        return Ok(favoritesWithDetails);
    }



    /// <summary>
    /// Adds a car to the user's favorites
    /// </summary>
    /// <param name="favorite"></param>
    [HttpPost]
    public async Task<IActionResult> AddFavorite(string userId, string carId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var car = await _context.Cars.FindAsync(Guid.Parse(carId));
        if (car == null)
        {
            return BadRequest("Car not found.");
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        var newFavorite = new Favorite
        {
            Id = Guid.NewGuid(),
            CarId = Guid.Parse( carId),
            UserId = userId,
            DateTime = DateTime.Now,
        };

        _context.Favorites.Add(newFavorite);
        await _context.SaveChangesAsync();

        return StatusCode(201);
    }

    /// <summary>
    /// Removes a car from the user's favorites
    /// </summary>
    /// <param name="id"></param>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFavorite(Guid id)
    {
        var favorite = await _context.Favorites.FindAsync(id);
        if (favorite == null)
        {
            return NotFound();
        }

        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
