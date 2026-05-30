using SharpLine.Api.Models;

namespace SharpLine.Api.Services.IService.IAuthService
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
    }
}
