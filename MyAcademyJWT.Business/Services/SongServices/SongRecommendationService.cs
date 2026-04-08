using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using MyAcademyJWT.Business.DTOs.SongDtos;
using MyAcademyJWT.Business.MLModels;
using MyAcademyJWT.DataAccess.Context;

namespace MyAcademyJWT.Business.Services.SongServices
{
    public class SongRecommendationService : ISongRecommendationService
    {
        private readonly AppDbContext _context;
        private readonly MLContext _mlContext;
        private static ITransformer _trainedModel;

        public SongRecommendationService(AppDbContext context)
        {
            _context = context;
            _mlContext = new MLContext();
        }

        public async Task TrainModelAsync()
        {
            var histories = await _context.UserSongHistories.ToListAsync();

            var trainingData = histories
                .GroupBy(h => new { h.AppUserId, h.SongId })
                .Select(g => new UserSongData
                {
                    UserId = (float)g.Key.AppUserId,
                    SongId = (float)g.Key.SongId,
                    Label = g.Count()
                }).ToList();

            if (!trainingData.Any()) return;

            IDataView dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "userIdEncoded", inputColumnName: nameof(UserSongData.UserId))
                .Append(_mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "songIdEncoded", inputColumnName: nameof(UserSongData.SongId)))
                .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(
                    new MatrixFactorizationTrainer.Options
                    {
                        MatrixColumnIndexColumnName = "userIdEncoded",
                        MatrixRowIndexColumnName = "songIdEncoded",
                        LabelColumnName = nameof(UserSongData.Label),
                        NumberOfIterations = 20,
                        ApproximationRank = 100
                    }));

            _trainedModel = pipeline.Fit(dataView);
        }

        public async Task<List<SongListDto>> GetRecommendationsForUserAsync(int userId, int count = 6) 
        {
            if (_trainedModel == null)
            {
                await TrainModelAsync();
            }

            if (_trainedModel == null) return new List<SongListDto>();

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<UserSongData, SongPrediction>(_trainedModel);

            // Dinlenen Şarkıları Bul
            var listenedSongIds = await _context.UserSongHistories
                .Where(h => h.AppUserId == userId)
                .Select(h => h.SongId)
                .Distinct()
                .ToListAsync();

            // Son 15 Dinleme Geçmişindeki Sanatçıları Bul
            var recentHistoryArtists = await _context.UserSongHistories
                .Include(h => h.Song)
                    .ThenInclude(s => s.Album)
                        .ThenInclude(a => a.Artist)
                .Where(h => h.AppUserId == userId)
                .OrderByDescending(h => h.Id)
                .Take(15)
                .Select(h => h.Song.Album.Artist.Name)
                .Distinct()
                .ToListAsync();

            // 3. Dinlenmemiş Şarkıları Getir
            var unlistenedSongs = await _context.Songs
                .Include(s => s.Album)
                    .ThenInclude(a => a.Artist)
                .Where(s => !listenedSongIds.Contains(s.Id))
                .ToListAsync();

            // Tüm tahminleri tutacağımız liste
            var predictions = new List<Tuple<MyAcademyJWT.Entity.Entities.Song, float>>();

            foreach (var song in unlistenedSongs)
            {
                var prediction = predictionEngine.Predict(new UserSongData
                {
                    UserId = userId,
                    SongId = song.Id
                });

                // Eğer şarkının sanatçısı son 15'te varsa, ML.NET puanına BONUS ekle
                float finalScore = prediction.Score;
                if (recentHistoryArtists.Contains(song.Album?.Artist?.Name))
                {
                    finalScore += 0.5f; // Son dinlenilenlere %50 daha fazla ağırlık
                }

                predictions.Add(new Tuple<MyAcademyJWT.Entity.Entities.Song, float>(song, finalScore));
            }

            var topDiverseSongs = predictions
                .GroupBy(p => p.Item1.Album?.Artist?.Name ?? "Bilinmeyen") // Önce Sanatçıya göre grupla
                .SelectMany(g => g.OrderByDescending(p => p.Item2).Take(2)) // Her sanatçıdan EN FAZLA 2 şarkı havuza girsin!
                .OrderByDescending(p => p.Item2) // Çeşitlendirilmiş listeyi puana göre diz
                .Take(30) // En iyi 30'u al
                .Select(p => p.Item1) // Sadece Şarkı nesnelerini al
                .OrderBy(x => Guid.NewGuid()) // Bu 30'u kendi içinde iyice karıştır
                .Take(count) // Ekrana basılacak kadarını (6 adet) al
                .ToList();

            // 5. Şarkıları DTO'ya çevir
            var recommendedSongs = topDiverseSongs.Select(s => new SongListDto
            {
                Id = s.Id,
                Title = s.Title,
                ArtistName = s.Album?.Artist?.Name ?? "Bilinmeyen Sanatçı",
                AlbumTitle = s.Album?.Title ?? "Bilinmeyen Albüm",
                CoverImageUrl = s.Album?.CoverImageUrl,
                Duration = s.Duration.ToString(@"mm\:ss"),
                RequiredContentLevel = s.RequiredContentLevel
            }).ToList();

            return recommendedSongs;
        }

        public static void ForceRetrain()
        {
            _trainedModel = null;
        }
    }
}
