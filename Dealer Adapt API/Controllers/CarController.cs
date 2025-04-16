using Dealer_Adapt_API.Data;
using Dealer_Adapt_API.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dealer_Adapt_API.Controllers;

/// <summary>
/// The CarController class that inherits from ControllerBase and is used to interact with the Car entity model
/// It contains methods to perform CRUD operations on the Car entity
/// The route for this controller is set to api/[controller]
/// 
/// ••• Summary •••
/// •	GetCar: Retrieves a car by its ID.
/// •	GetAllCars: Retrieves all cars.
/// •	AddCar: Adds a new car to the database.
/// •	UpdateCar: Updates an existing car.
/// •	DeleteCar: Deletes a car by its ID.
/// </summary>
[Route("api/[controller]")]
[ApiController]
//[Authorize(Roles = "Salesman,BusinessOwner")]
public class CarController : ControllerBase
{
    /// <summary>
    /// The ApplicationDbContext instance to interact with the database
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Constructor for the CarController class that takes an ApplicationDbContext instance as a parameter
    /// </summary>
    /// <param name="context"></param>
    public CarController(ApplicationDbContext context)
    {
        _context = context;
    }




    // ROLE................ No Role Required ................

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
            car.CoverImageId,
            car.Year,
            car.IsNew,
            //PictureUrls = car.Pictures?.Select(p => p.Url).ToList() // Get the Picture URLs
            Pictures = car.Pictures?.Select(p => new { Id = p.Id, Url = p.Url }).ToList()
        };

        return Ok(carWithPictures);
    }




    ///// <summary>
    ///// Retrieves all cars from the database and returns them as an IActionResult object
    ///// </summary>
    ///// <returns></returns>
    //[HttpGet]
    //public async Task<IActionResult> GetAllCars()
    //{
    //    var cars = await _context.Cars
    //    .Include(c => c.Pictures) // Include the Pictures navigation property
    //    .ToListAsync();

    //    var carsWithPictures = cars.Select(car => new
    //    {
    //        car.Id,
    //        car.Make,
    //        car.Model,
    //        car.Mileage,
    //        car.Price,
    //        car.Color,
    //        car.VIN,
    //        car.Description,
    //        car.CoverImageId,
    //        car.Year,
    //        car.IsNew,
    //        //PictureUrls = car.Pictures?.Select(p => p.Url).ToList() // Get the Picture URLs
    //        Pictures = car.Pictures?.Select(p => new { Id = p.Id, Url = p.Url }).ToList()
    //    }).ToList();

    //    return Ok(carsWithPictures);
    //}





    // Methods Must be Role................ Salesman or Business Owner Required ................

    /// <summary>
    /// Adds a new car to the database along with multiple pictures
    /// </summary>

    [HttpPost]
    public async Task<IActionResult> AddCar([FromForm] Car car, [FromForm] List<IFormFile> imageFiles)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (imageFiles.Count > 25)
        {
            return BadRequest("You can upload up to 25 images.");
        }

        var newCar = new Car
        {
            Id = Guid.NewGuid(),
            Make = car.Make,
            Model = car.Model,
            Mileage = car.Mileage,
            Price = car.Price,
            Color = car.Color,
            VIN = car.VIN,
            Description = car.Description,
            Year = car.Year,
            IsNew = car.IsNew,
        };

        _context.Cars.Add(newCar);

        // Handle picture files if provided
        if (imageFiles != null && imageFiles.Any())
        {
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CarImages");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var random = new Random();

            foreach (var file in imageFiles)
            {
                if (file.Length > 0 && file.Length <= 5 * 1024 * 1024) // Check file size up to 5MB
                {
                    var randomInteger = random.Next(0, Int32.MaxValue);
                    var fileName = $"{newCar.Year}-{newCar.Make}-{newCar.Model}-{newCar.Mileage}-{newCar.VIN}-{randomInteger}{Path.GetExtension(file.FileName)}";
                    //var fileName = $"{randomInteger}-{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    //Car car = await _context.Cars.FindAsync(Guid.Parse("25d584c2-d871-43bc-c975-08dcf8b721c5"));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var pictureUrl = $"/CarImages/{fileName}";

                    Picture picture = new Picture
                    {
                        Id = Guid.NewGuid(),
                        Url = pictureUrl,
                        //CarId = Guid.Parse("25d584c2-d871-43bc-c975-08dcf8b721c5"),
                        //Car = car
                        CarId = newCar.Id,
                        Car = newCar
                    };
                    _context.Pictures.Add(picture);
                }
                else
                {
                    return BadRequest("Each image file must be up to 5MB in size.");
                }
            }
        }

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCar), new { id = newCar.Id }, newCar);
        //return Ok();
    }




    /// <summary>
    /// Updates an existing car in the database based on the ID provided in the request URL
    /// </summary>
    /// <param name="id"></param>
    /// <param name="car"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCar(Guid id, [FromBody] Car car)
    {
        if (id != car.Id)
        {
            return BadRequest("Car ID mismatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingCar = await _context.Cars.FindAsync(id);
        if (existingCar == null)
        {
            return NotFound();
        }

        existingCar.Make = car.Make;
        existingCar.Model = car.Model;
        existingCar.Mileage = car.Mileage;
        existingCar.Price = car.Price;
        existingCar.Color = car.Color;
        existingCar.VIN = car.VIN;
        existingCar.Description = car.Description;
        existingCar.Year = car.Year;
        existingCar.IsNew = car.IsNew;

        // Handle picture URLs if provided
        if (car.ListOfPictureUrls != null && car.ListOfPictureUrls.Any())
        {
            // Remove existing pictures that are not in car.ListOfPictureUrls
            var existingPictures = _context.Pictures.Where(p => p.CarId == id && !car.ListOfPictureUrls.Contains(p.Url)).ToList();
            _context.Pictures.RemoveRange(existingPictures);


            // Add new pictures that are not already in the database
            foreach (var url in car.ListOfPictureUrls)
            {
                if (!_context.Pictures.Any(p => p.Url == url))
                {
                    Picture picture = new Picture
                    {
                        Id = Guid.NewGuid(),
                        Url = url,
                        CarId = id,
                        Car = existingCar
                    };
                    _context.Pictures.Add(picture);
                }
            }
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }




    /// <summary>
    /// Deletes a car from the database based on the ID provided in the request URL
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCar(Guid id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null)
        {
            return NotFound();
        }

        _context.Cars.Remove(car);
        await _context.SaveChangesAsync();

        return NoContent();
    }



    /// <summary>
    /// Sets the cover image for a car
    /// </summary>
    /// <param name="id"></param>
    /// <param name="coverImageId"></param>
    /// <returns></returns>
    [HttpPut("{id}/cover-image")]
    public async Task<IActionResult> SetCoverImage(Guid id, [FromBody] Guid coverImageId)
    {
        // Find the car
        var car = await _context.Cars.FindAsync(id);
        if (car == null)
        {
            return NotFound();
        }

        // Find the picture
        var picture = await _context.Pictures.FindAsync(coverImageId);
        // Check if the picture exists and belongs to the car
        if (picture == null || picture.CarId != id)
        {
            return BadRequest("Invalid cover image ID");
        }

        // Set the cover image for the car and save changes
        car.CoverImageId = coverImageId;
        await _context.SaveChangesAsync();

        return NoContent();
    }



    /// <summary>
    /// Retrieves the picture URL by coverImageId
    /// </summary>
    /// <param name="coverImageId"></param>
    /// <returns></returns>
    [HttpGet("cover-image/{coverImageId}")]
    public async Task<IActionResult> GetPictureUrlByCoverImageId(Guid coverImageId)
    {
        var picture = await _context.Pictures
            .FirstOrDefaultAsync(p => p.Id == coverImageId);

        if (picture == null)
        {
            return NotFound();
        }

        return Ok(new { picture.Url });
    }
}
