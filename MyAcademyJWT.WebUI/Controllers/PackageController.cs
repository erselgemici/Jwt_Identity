using Microsoft.AspNetCore.Mvc;
using MyAcademyJWT.WebUI.Models.Package;
using System.Text.Json;

namespace MyAcademyJWT.WebUI.Controllers
{
    public class PackageController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PackageController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();

            var responseMessage = await client.GetAsync("https://localhost:7074/api/Packages");

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var packages = JsonSerializer.Deserialize<List<ResultPackageDto>>(jsonData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                packages = packages?.OrderBy(p => p.ContentLevel).ToList();

                return View(packages);
            }

            return View(new List<ResultPackageDto>());
        }

        [HttpPost]
        public async Task<IActionResult> Buy(int packageId)
        {
            var client = _httpClientFactory.CreateClient();
            var token = Request.Cookies["JwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth"); 
            }

            // API'ye yetkili (Bearer) istek yapıyoruz
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var responseMessage = await client.PostAsync($"https://localhost:7074/api/Packages/buy/{packageId}", null);

            if (responseMessage.IsSuccessStatusCode)
            {
                Response.Cookies.Delete("JwtToken");

                TempData["SuccessMessage"] = "Paketiniz başarıyla yükseltildi! Yeni özelliklerinizi kullanmak için lütfen tekrar giriş yapın. 👑";
                return RedirectToAction("Login", "Auth");
            }

            TempData["ErrorMessage"] = "İşlem sırasında bir hata oluştu.";
            return RedirectToAction("Index");
        }
    }
}
