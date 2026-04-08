using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAcademyJWT.Business.Services.DeezerServices;
using MyAcademyJWT.Business.Services.SongServices;
using System.Security.Claims;

namespace MyAcademyJWT_Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SongsController : ControllerBase
    {
        private readonly ISongService _songService;
        private readonly IDeezerService _deezerService;
        private readonly ISongRecommendationService _recommendationService;

        public SongsController(ISongService songService, IDeezerService deezerService, ISongRecommendationService recommendationService)
        {
            _songService = songService;
            _deezerService = deezerService;
            _recommendationService = recommendationService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllSongs()
        {
            var songs = await _songService.GetAllSongsAsync();
            return Ok(songs);
        }

        [HttpGet("{id}/play")]
        public async Task<IActionResult> PlaySong(int id)
        {
            var song = await _songService.GetSongForPlayAsync(id);

            if (song == null)
            {
                return NotFound(new { message = "Şarkı bulunamadı." });
            }

            var packageClaim = User.Claims.FirstOrDefault(c => c.Type == "PackageId");
            if (packageClaim == null)
            {
                return Unauthorized(new { message = "Token içinde paket bilgisi bulunamadı." });
            }

            int userPackageLevel = Convert.ToInt32(packageClaim.Value);

            if (userPackageLevel > song.RequiredContentLevel)
            {
                return StatusCode(403, new
                {
                    message = "Bu şarkıyı dinlemek için paket seviyenizi yükseltmelisiniz!",
                    requiredLevel = song.RequiredContentLevel,
                    yourLevel = userPackageLevel
                });
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                await _songService.RecordSongPlayAsync(userId, id);
            }

            return Ok(song);
        }

        [HttpGet("trending")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTrendingSongs()
        {
            var songs = await _songService.GetTopTrendingSongsAsync(10);
            return Ok(songs);
        }

        [HttpGet("recently-played")]
        public async Task<IActionResult> GetRecentlyPlayed()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            var songs = await _songService.GetUserRecentlyPlayedAsync(userId, 5);
            return Ok(songs);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var songs = await _songService.SearchSongsAsync(keyword);
            return Ok(songs);
        }

        [HttpPost("seed-from-deezer")]
        [AllowAnonymous]
        public async Task<IActionResult> SeedDeezer([FromQuery] string query = "Türkçe Pop")
        {
            var result = await _deezerService.SeedTracksFromDeezerAsync(query);
            return Ok(result);
        }

        [HttpGet("recommendations")]
        [Authorize] 
        public async Task<IActionResult> GetRecommendations()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            var recommendedSongs = await _recommendationService.GetRecommendationsForUserAsync(userId, 6);
            return Ok(recommendedSongs);
        }

        [HttpGet("{id}")]
        [Authorize] 
        public async Task<IActionResult> GetSongForPlayer(int id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            var song = await _songService.GetSongForPlayAsync(id);

            if (song == null)
            {
                return NotFound("Şarkı bulunamadı.");
            }

            await _songService.RecordSongPlayAsync(userId, id);

            // Yapay Zekanın hafızasını sıfırla. Böylece ana sayfaya döndüğünde yeni zevklerine göre öneri yapacak.
            SongRecommendationService.ForceRetrain();

            return Ok(song);
        }
    }
}
