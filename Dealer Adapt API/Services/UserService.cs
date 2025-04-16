using Dealer_Adapt_API.Data;
using Dealer_Adapt_API.Models.IdentityUserModels;
using Microsoft.EntityFrameworkCore;

namespace Dealer_Adapt_API.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
    }



    public Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        // Implement your password reset logic here
        return Task.FromResult(true);
    }
}

