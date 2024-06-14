using Microsoft.AspNetCore.Identity;

namespace BC_Web2.Models.Interfaces
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}

