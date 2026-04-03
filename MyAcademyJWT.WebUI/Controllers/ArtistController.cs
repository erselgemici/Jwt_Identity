using Microsoft.AspNetCore.Mvc;
using MyAcademyJWT.WebUI.Models.Artist;
using MyAcademyJWT.WebUI.Models.Song;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MyAcademyJWT.WebUI.Controllers
{
    public class ArtistController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ArtistController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var artists = new List<ResultArtistDto>();

            var topSongs = new List<ResultSongDto>();

            var token = HttpContext.Request.Cookies["JwtToken"];
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            try
            {
                var artistResponse = await client.GetAsync("https://localhost:7074/api/Artists");
                if (artistResponse.IsSuccessStatusCode)
                {
                    var jsonData = await artistResponse.Content.ReadAsStringAsync();
                    artists = JsonSerializer.Deserialize<List<ResultArtistDto>>(jsonData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    artists = artists?.OrderBy(a => a.Name).ToList() ?? new List<ResultArtistDto>();
                }

               
                var songResponse = await client.GetAsync("https://localhost:7074/api/Songs");

                if (songResponse.IsSuccessStatusCode)
                {
                    var songJson = await songResponse.Content.ReadAsStringAsync();
                    var allSongs = JsonSerializer.Deserialize<List<ResultSongDto>>(songJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (allSongs != null && allSongs.Any())
                    {
                        topSongs = allSongs.OrderBy(x => Guid.NewGuid()).Take(5).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("API Hatası: " + ex.Message);
            }

            ViewBag.TopSongs = topSongs;
            return View(artists);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id, int page = 1, string search = null)
        {
            var client = _httpClientFactory.CreateClient();

            var token = HttpContext.Request.Cookies["JwtToken"];
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var artistResponse = await client.GetAsync($"https://localhost:7074/api/Artists/{id}");
            if (!artistResponse.IsSuccessStatusCode) return RedirectToAction("Index");

            var artistJson = await artistResponse.Content.ReadAsStringAsync();
            var artist = JsonSerializer.Deserialize<ResultArtistDto>(artistJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var songs = new List<ResultSongDto>();

            var encodedArtistName = Uri.EscapeDataString(artist.Name);
            var songResponse = await client.GetAsync($"https://localhost:7074/api/Songs/search?keyword={encodedArtistName}");

            if (songResponse.IsSuccessStatusCode)
            {
                var songJson = await songResponse.Content.ReadAsStringAsync();
                songs = JsonSerializer.Deserialize<List<ResultSongDto>>(songJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ResultSongDto>();
            }
            else if (songResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
               
                return RedirectToAction("Index", "Login");
            }

            if (!string.IsNullOrEmpty(search))
            {
                songs = songs.Where(s => s.Title.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                ViewBag.CurrentSearch = search;
            }

            // Pagination
            int pageSize = 5;
            int totalSongs = songs.Count;
            int totalPages = (int)Math.Ceiling(totalSongs / (double)pageSize);

            if (totalPages == 0) totalPages = 1;

            var pagedSongs = songs.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.ArtistSongs = pagedSongs;
            ViewBag.TotalSongs = totalSongs;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(artist);
        }
    }
}
