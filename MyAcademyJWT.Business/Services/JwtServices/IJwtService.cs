using MyAcademyJWT.Entity.Entities;

namespace MyAcademyJWT.Business.Services.JwtServices
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(AppUser user);
    }
}
