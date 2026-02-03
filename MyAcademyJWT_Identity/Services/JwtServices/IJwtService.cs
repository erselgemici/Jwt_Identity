using MyAcademyJWT_Identity.Entities;

namespace MyAcademyJWT_Identity.Services.JwtServices
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(AppUser user);
    }
}
