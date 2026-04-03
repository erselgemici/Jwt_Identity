using Microsoft.EntityFrameworkCore;
using MyAcademyJWT.DataAccess.Context;
using MyAcademyJWT.Entity.Entities;
using System.Text.Json;

namespace MyAcademyJWT.Business.Services.DeezerServices
{
    public class DeezerService : IDeezerService
    {
        private readonly AppDbContext _context;

        public DeezerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> SeedTracksFromDeezerAsync(string searchQuery)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"https://api.deezer.com/search?q={searchQuery}");

            if (!response.IsSuccessStatusCode)
                return "Deezer API'ye ulaşılamadı.";

            var jsonString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonString);
            var data = doc.RootElement.GetProperty("data");

            int addedCount = 0;
            var random = new Random();
            int[] availablePackages = { 1, 3, 6 }; // Elite, Gold, Free

            foreach (var item in data.EnumerateArray())
            {
                var previewUrl = item.GetProperty("preview").GetString();
                if (string.IsNullOrEmpty(previewUrl)) continue; // Mp3 önizlemesi yoksa atla

                // DEĞİŞİKLİK 1: Şarkının Deezer'daki orijinal ID'sini alıyoruz
                var deezerId = item.GetProperty("id").GetInt64();

                // Sanatçı
                var artistName = item.GetProperty("artist").GetProperty("name").GetString();
                var artist = await _context.Artists.FirstOrDefaultAsync(a => a.Name == artistName);
                if (artist == null)
                {
                    var artistPic = item.GetProperty("artist").GetProperty("picture_xl").GetString();
                    artist = new Artist { Name = artistName, Country = "Global", ImageUrl = artistPic };
                    await _context.Artists.AddAsync(artist);
                    await _context.SaveChangesAsync();
                }

                // Albüm
                var albumTitle = item.GetProperty("album").GetProperty("title").GetString();
                var album = await _context.Albums.FirstOrDefaultAsync(a => a.Title == albumTitle && a.ArtistId == artist.Id);
                if (album == null)
                {
                    var coverUrl = item.GetProperty("album").GetProperty("cover_xl").GetString();
                    album = new Album { Title = albumTitle, ReleaseYear = DateTime.Now.Year, ArtistId = artist.Id, CoverImageUrl = coverUrl };
                    await _context.Albums.AddAsync(album);
                    await _context.SaveChangesAsync();
                }

                var songExists = await _context.Songs.AnyAsync(s => s.DeezerTrackId == deezerId);
                if (!songExists)
                {
                    var durationSeconds = item.GetProperty("duration").GetInt32();
                    var songTitle = item.GetProperty("title").GetString();

                    var newSong = new Song
                    {
                        Title = songTitle,
                        AlbumId = album.Id,
                        DeezerTrackId = deezerId, 
                        Duration = TimeSpan.FromSeconds(durationSeconds),
                        RequiredContentLevel = availablePackages[random.Next(availablePackages.Length)]
                    };

                    await _context.Songs.AddAsync(newSong);
                    addedCount++;
                }
            }

            await _context.SaveChangesAsync();
            return $"{addedCount} adet GERÇEK ŞARKI başarıyla eklendi!";
        }
    }
}
