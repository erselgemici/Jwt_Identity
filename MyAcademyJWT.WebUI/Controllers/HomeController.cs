using Microsoft.AspNetCore.Mvc;
using MyAcademyJWT.WebUI.Models.Song;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MyAcademyJWT.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var token = Request.Cookies["JwtToken"];

            var allSongs = new List<ResultSongDto>();
            var recentlyPlayed = new List<ResultSongDto>();
            var recommendations = new List<ResultSongDto>(); 

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // Son Dinlenenleri Çek
                var historyResponse = await client.GetAsync("https://localhost:7074/api/Songs/recently-played");
                if (historyResponse.IsSuccessStatusCode)
                {
                    var historyJson = await historyResponse.Content.ReadAsStringAsync();
                    recentlyPlayed = JsonSerializer.Deserialize<List<ResultSongDto>>(historyJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                // Yapay Zeka Önerilerini Çek
                var aiResponse = await client.GetAsync("https://localhost:7074/api/Songs/recommendations");
                if (aiResponse.IsSuccessStatusCode)
                {
                    var aiJson = await aiResponse.Content.ReadAsStringAsync();
                    recommendations = JsonSerializer.Deserialize<List<ResultSongDto>>(aiJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }

            // Tüm şarkıları çek
            var responseMessage = await client.GetAsync("https://localhost:7074/api/Songs");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                allSongs = JsonSerializer.Deserialize<List<ResultSongDto>>(jsonData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
         
                allSongs = allSongs?
                    .GroupBy(s => s.CoverImageUrl)
                    .Select(g => g.First())
                    .OrderBy(x => Guid.NewGuid())
                    .ToList();
            }

            ViewBag.RecentlyPlayed = recentlyPlayed;
            ViewBag.Recommendations = recommendations;

            return View(allSongs ?? new List<ResultSongDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Player(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var token = Request.Cookies["JwtToken"];

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var responseMessage = await client.GetAsync($"https://localhost:7074/api/Songs/{id}/play");

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var song = JsonSerializer.Deserialize<SongPlayDto>(jsonData, options);

                return View(song);
            }

            // Kullanıcının geldiği sayfayı (Referer) bulalım ki hatayı verip onu aynı sayfada tutalım
            string refererUrl = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(refererUrl))
            {
                refererUrl = "/Artist/Index";
            }

            // Eğer Paket Yetersizse (403 Forbidden)
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                // API'nin gönderdiği JSON hata mesajını yakalıyoruz
                var errorJson = await responseMessage.Content.ReadAsStringAsync();
                try
                {
                    var errorData = JsonSerializer.Deserialize<JsonElement>(errorJson);
                    // API'den gelen mesajı TempData'ya atıyoruz
                    TempData["ErrorMessage"] = errorData.GetProperty("message").GetString();
                }
                catch
                {
                    TempData["ErrorMessage"] = "Bu premium içeriği dinlemek için paketinizi yükseltmelisiniz 👑";
                }

                return Redirect(refererUrl);
            }

            if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                TempData["ErrorMessage"] = "Şarkıları dinleyebilmek için lütfen giriş yapın.";
                return RedirectToAction("Index", "Auth");
            }

            TempData["ErrorMessage"] = "Şarkı yüklenirken bir sorun oluştu.";
            return Redirect(refererUrl);
        }

        // Javascript'in arka planda şarkıyı çekip oynatması için
        [HttpGet]
        public async Task<IActionResult> GetSongForPlayer(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var token = Request.Cookies["JwtToken"];

            if (string.IsNullOrEmpty(token)) return Unauthorized();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // API'ye gidiyoruz (Bu sayede API tarafındaki geçmişe kaydetme 'History' metodu da tetiklenmiş oluyor)
            var response = await client.GetAsync($"https://localhost:7074/api/Songs/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return Content(json, "application/json"); // Şarkı verilerini JSON olarak JS'e yolla
            }

            // Eğer paket yetkisi yetmiyorsa (403 Forbidden vb)
            return StatusCode((int)response.StatusCode);
        }

        public async Task<IActionResult> Discover()
        {
            var client = _httpClientFactory.CreateClient();
            var allSongs = new List<ResultSongDto>();

            var responseMessage = await client.GetAsync("https://localhost:7074/api/Songs");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var songs = JsonSerializer.Deserialize<List<ResultSongDto>>(jsonData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (songs != null)
                {
                    allSongs = songs.OrderBy(x => Guid.NewGuid()).ToList();
                }
            }
            return View(allSongs);
        }
    }
}
