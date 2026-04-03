namespace MyAcademyJWT.Business.Services.DeezerServices
{
    public interface IDeezerService
    {
        Task<string> SeedTracksFromDeezerAsync(string searchQuery);
    }
}
