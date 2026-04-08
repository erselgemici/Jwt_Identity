using MyAcademyJWT.Business.DTOs;
using MyAcademyJWT.Business.DTOs.SongDtos;

namespace MyAcademyJWT.Business.Services.SongServices
{
    public interface ISongRecommendationService
    {
        // Modeli eğitmek için kullanacağımız metot
        Task TrainModelAsync();

        // Kullanıcıya özel şarkı önerecek metot
        Task<List<SongListDto>> GetRecommendationsForUserAsync(int userId, int count = 5);
    }
}
