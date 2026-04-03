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

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var responseMessage = await client.GetAsync("https://localhost:7074/api/Songs");

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var songs = JsonSerializer.Deserialize<List<ResultSongDto>>(jsonData, options);

                return View(songs);
            }

            return View(new List<ResultSongDto>());
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
    }
}
