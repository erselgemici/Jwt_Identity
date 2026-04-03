using MyAcademyJWT.Business.DTOs.ArtistDtos;

namespace MyAcademyJWT.Business.Services.ArtistServices
{
    public interface IArtistService
    {
        Task<List<ResultArtistDto>> GetAllArtistsAsync();
        Task<ResultArtistDto> GetArtistByIdAsync(int id);
    }
}
