using Dealer_Adapt_API.Models.IdentityUserModels;

namespace Dealer_Adapt_API.Services;

public interface IUserService
{
    Task<ApplicationUser?> GetUserByEmailAsync(string email);

    Task<bool> ResetPasswordAsync(string token, string newPassword);
}
