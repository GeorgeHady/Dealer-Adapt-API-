//using Dealer_Adapt_API.Migrations;
using Dealer_Adapt_API.Models.Entities;
using Dealer_Adapt_API.Models.IdentityUserModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Net.Mime.MediaTypeNames;

namespace Dealer_Adapt_API.Data;

/// <summary>
/// The ApplicationDbContext class that inherits from IdentityDbContext<ApplicationUser> and is used to interact with the database
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    // Constructor for the ApplicationDbContext class that takes DbContextOptions as a parameter
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// Define your DbSets here ///

    /// <summary>
    /// DbSet for the Car entity model class to interact with the database
    /// </summary>
    public DbSet<Car> Cars { get; set; }

    /// <summary>
    /// DbSet for the Picture entity model class to interact with the database
    /// </summary>
    public DbSet<Picture> Pictures { get; set; }

    /// <summary>
    /// DbSet for the Favorite entity model class to interact with the database
    /// </summary>
    public DbSet<Favorite> Favorites { get; set; }


    /// <summary>
    /// Override the OnModelCreating method to configure the relationships between the entities
    /// • Ensure that the Car entity has a navigation property for the cover image.
    /// • Ensure that the Picture entity has a navigation property back to the Car for the cover image.
    /// •• One-to-Many Relationship: A Car can have multiple Pictures, and each Picture is associated with one Car via the CarId foreign key.
    /// •• One-to-One Relationship: A Car has one Picture as its cover image, identified by the CoverImageId foreign key.The OnDelete(DeleteBehavior.Restrict) ensures that you cannot delete a Picture if it is set as the cover image for a Car.
    /// </summary>
    /// <param name = "modelBuilder" ></ param >
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure one-to-many relationship between Car and Picture
        modelBuilder.Entity<Car>()
            .HasMany(c => c.Pictures)
            .WithOne(p => p.Car)
            .HasForeignKey(p => p.CarId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-one relationship for CoverImageId
        //modelBuilder.Entity<Car>()
        //    .HasOne(c => c.Picture)
        //    .WithMany()
        //    .HasForeignKey(c => c.CoverImageId)
        //    .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Car>()
          .HasOne(c => c.Picture)
          .WithOne()
          .HasForeignKey<Car>(c => c.CoverImageId)
          .OnDelete(DeleteBehavior.Restrict);


        // Configure one-to-many relationship between ApplicationUser and Favorite
        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.Favorites)
            .WithOne(f => f.User)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-many relationship between Car and Favorite
        modelBuilder.Entity<Car>()
            .HasMany(c => c.Favorites)
            .WithOne(f => f.Car)
            .HasForeignKey(f => f.CarId)
            .OnDelete(DeleteBehavior.Cascade);
    }


}
