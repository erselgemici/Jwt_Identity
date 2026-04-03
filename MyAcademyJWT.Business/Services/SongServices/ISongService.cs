using MyAcademyJWT.Business.DTOs.SongDtos;

namespace MyAcademyJWT.Business.Services.SongServices
{
    public interface ISongService
    {
        Task<List<SongListDto>> GetAllSongsAsync();
        Task<SongPlayDto> GetSongForPlayAsync(int songId);
        Task RecordSongPlayAsync(int userId, int songId);
        Task<List<SongListDto>> GetTopTrendingSongsAsync(int count = 10);
        Task<List<SongListDto>> GetUserRecentlyPlayedAsync(int userId, int count = 5);
        Task<List<SongListDto>> SearchSongsAsync(string keyword);
    }
}
