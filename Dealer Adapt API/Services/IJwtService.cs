using Dealer_Adapt_API.Models.IdentityUserModels;
using System.Security.Claims;

namespace Dealer_Adapt_API.Services
{
    public interface IJwtService
    {
        string GenerateJwtToken(ApplicationUser user, string role);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}

