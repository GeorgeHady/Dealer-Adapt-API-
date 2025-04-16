using Dealer_Adapt_API.Models.Entities;
using Dealer_Adapt_API.Models.IdentityUserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dealer_Adapt_API.Data;

/// <summary>
/// Initializes the database with default roles and users
/// </summary>
public class DbInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public DbInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }



    /// <summary>
    /// Seeds the database with default roles and users on application start
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await SeedRolesAsync(roleManager);
            await SeedUserAsync(userManager);
            await SeedCarDataAsync(dbContext);

        }
    }


    /// <summary>
    /// No action needed on stop for this service
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        // No action needed on stop
        return Task.CompletedTask;
    }



    /// <summary>
    /// Seeds the database with default roles
    /// </summary>
    /// <param name="roleManager"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = ["Customer", "Salesman", "BusinessOwner"];
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!roleResult.Succeeded)
                {
                    throw new Exception($"Failed to create role {roleName}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }
        }
    }


    /// <summary>
    /// Seeds the database with default user
    /// </summary>
    /// <param name="userManager"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task SeedUserAsync(UserManager<ApplicationUser> userManager)
    {
        var defaultUser = new ApplicationUser
        {
            UserName = "user@example.com",
            Email = "user@example.com",
            FirstName = "Owner",
            EmailConfirmed = true
        };

        var user = await userManager.FindByEmailAsync(defaultUser.Email);
        if (user == null)
        {
            var createUserResult = await userManager.CreateAsync(defaultUser, "sS!1tring");
            if (!createUserResult.Succeeded)
            {
                throw new Exception($"Failed to create default user: {string.Join(", ", createUserResult.Errors.Select(e => e.Description))}");
            }

            var addToRoleResult = await userManager.AddToRoleAsync(defaultUser, "BusinessOwner");
            if (!addToRoleResult.Succeeded)
            {
                throw new Exception($"Failed to add default user to Admin role: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
            }
        }
    }



    /// <summary>
    /// Seeds the database with car data
    /// </summary>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task SeedCarDataAsync(ApplicationDbContext dbContext)
    {
        if (!dbContext.Cars.Any())
        {
            var cars = Enumerable.Range(1, 100).Select(i => new Car
            {
                Make = $"Make{i % 10}",
                Model = $"Model{i % 20}",
                Year = 2000 + (i % 21),
                Color = $"Color{i % 5}",
                Price = 10000 + (i * 100)
            }).ToList();

            await dbContext.Cars.AddRangeAsync(cars);
            await dbContext.SaveChangesAsync();
        }
    }
}
