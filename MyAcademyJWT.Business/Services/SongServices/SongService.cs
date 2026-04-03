using Microsoft.EntityFrameworkCore;
using MyAcademyJWT.Business.DTOs.SongDtos;
using MyAcademyJWT.DataAccess.Context;
using MyAcademyJWT.Entity.Entities;
using System.Net.Http;       // Deezer API isteği için eklendi
using System.Text.Json;      // JSON parçalamak için eklendi

namespace MyAcademyJWT.Business.Services.SongServices
{
    public class SongService : ISongService
    {
        private readonly AppDbContext _context;

        public SongService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SongListDto>> GetAllSongsAsync()
        {
            var songs = await _context.Songs
                .Include(s => s.Album)
                .ThenInclude(a => a.Artist)
                .Select(s => new SongListDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    ArtistName = s.Album.Artist.Name,
                    AlbumTitle = s.Album.Title,
                    CoverImageUrl = s.Album.CoverImageUrl,
                    Duration = s.Duration.ToString(@"hh\:mm\:ss"),
                    RequiredContentLevel = s.RequiredContentLevel
                }).ToListAsync();

            return songs;
        }

        // DEĞİŞEN TEK METOT BURASI
        public async Task<SongPlayDto> GetSongForPlayAsync(int songId)
        {
            // 1. AŞAMA: Veritabanından şarkıyı ve DeezerTrackId'sini anonim obje olarak çekiyoruz
            var songEntity = await _context.Songs
                .Where(s => s.Id == songId)
                .Select(s => new
                {
                    s.Id,
                    s.Title,
                    s.DeezerTrackId, // Önceki adımlarda Entity'e eklediğimiz yeni kolon
                    s.RequiredContentLevel
                }).FirstOrDefaultAsync();

            if (songEntity == null) return null;

            string freshPreviewUrl = "";

            // 2. AŞAMA: Deezer API'den anlık taze mp3 linkini alıyoruz
            using (var httpClient = new HttpClient())
            {
                try
                {
                    string apiUrl = $"https://api.deezer.com/track/{songEntity.DeezerTrackId}";
                    var response = await httpClient.GetStringAsync(apiUrl);

                    using (var doc = JsonDocument.Parse(response))
                    {
                        if (doc.RootElement.TryGetProperty("preview", out var previewElement))
                        {
                            freshPreviewUrl = previewElement.GetString();
                        }
                    }
                }
                catch (Exception)
                {
                    // API çökmesi veya internet gitmesi durumları için loglama yapabilirsin
                }
            }

            // 3. AŞAMA: Taze linki (freshPreviewUrl) DTO'ya verip Controller'a gönderiyoruz
            return new SongPlayDto
            {
                Id = songEntity.Id,
                Title = songEntity.Title,
                AudioUrl = freshPreviewUrl, // Dinamik olarak üretilen link burada!
                RequiredContentLevel = songEntity.RequiredContentLevel
            };
        }

        public async Task<List<SongListDto>> GetTopTrendingSongsAsync(int count = 10)
        {
            var trendingSongs = await _context.UserSongHistories
                .Include(h => h.Song).ThenInclude(s => s.Album).ThenInclude(a => a.Artist)
                .GroupBy(h => h.Song)
                .OrderByDescending(g => g.Count())
                .Take(count)
                .Select(g => new SongListDto
                {
                    Id = g.Key.Id,
                    Title = g.Key.Title,
                    ArtistName = g.Key.Album.Artist.Name,
                    AlbumTitle = g.Key.Album.Title,
                    CoverImageUrl = g.Key.Album.CoverImageUrl,
                    Duration = g.Key.Duration.ToString(@"hh\:mm\:ss"),
                    RequiredContentLevel = g.Key.RequiredContentLevel
                }).ToListAsync();

            return trendingSongs;
        }

        public async Task<List<SongListDto>> GetUserRecentlyPlayedAsync(int userId, int count = 5)
        {
            var recentSongs = await _context.UserSongHistories
                .Include(h => h.Song).ThenInclude(s => s.Album).ThenInclude(a => a.Artist)
                .Where(h => h.AppUserId == userId)
                .OrderByDescending(h => h.ListenedAt)
                .Take(count)
                .Select(h => new SongListDto
                {
                    Id = h.Song.Id,
                    Title = h.Song.Title,
                    ArtistName = h.Song.Album.Artist.Name,
                    AlbumTitle = h.Song.Album.Title,
                    CoverImageUrl = h.Song.Album.CoverImageUrl,
                    Duration = h.Song.Duration.ToString(@"hh\:mm\:ss"),
                    RequiredContentLevel = h.Song.RequiredContentLevel
                }).ToListAsync();

            return recentSongs;
        }

        public async Task RecordSongPlayAsync(int userId, int songId)
        {
            var history = new UserSongHistory
            {
                AppUserId = userId,
                SongId = songId,
                ListenedAt = DateTime.Now
            };

            await _context.UserSongHistories.AddAsync(history);
            await _context.SaveChangesAsync();
        }

        public async Task<List<SongListDto>> SearchSongsAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return new List<SongListDto>();

            var keywordLower = keyword.ToLower();

            var searchResults = await _context.Songs
                .Include(s => s.Album).ThenInclude(a => a.Artist)
                .Where(s => s.Title.ToLower().Contains(keywordLower) ||
                            s.Album.Title.ToLower().Contains(keywordLower) ||
                            s.Album.Artist.Name.ToLower().Contains(keywordLower))
                .Select(s => new SongListDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    ArtistName = s.Album.Artist.Name,
                    AlbumTitle = s.Album.Title,
                    CoverImageUrl = s.Album.CoverImageUrl,
                    Duration = s.Duration.ToString(@"hh\:mm\:ss"),
                    RequiredContentLevel = s.RequiredContentLevel
                }).ToListAsync();

            return searchResults;
        }
    }
}
