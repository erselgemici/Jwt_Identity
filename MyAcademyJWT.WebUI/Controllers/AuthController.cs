using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MyAcademyJWT.WebUI.Models.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace MyAcademyJWT.WebUI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            var client = _httpClientFactory.CreateClient();

            // Veriyi JSON formatına çeviriyoruz
            var jsonData = JsonSerializer.Serialize(model);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var responseMessage = await client.PostAsync("https://localhost:7074/api/Users/login", stringContent);

            if (responseMessage.IsSuccessStatusCode)
            {
                var responseContent = await responseMessage.Content.ReadAsStringAsync();

                // API'den gelen JSON içindeki Token'ı okuyoruz
                using var jsonDoc = JsonDocument.Parse(responseContent);
                string token = jsonDoc.RootElement.GetProperty("token").GetString();

                // Token'ı API isteklerinde kullanmak üzere Cookie'ye kaydediyoruz
                Response.Cookies.Append("JwtToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddDays(1)
                });

                // Token'ın içini açıp kullanıcının bilgilerini MVC'ye tanıtıyoruz
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var claims = new List<Claim>();
                claims.AddRange(jwtToken.Claims); // API'den gelen isim, rol, ID gibi tüm bilgileri aldık

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = true }; // Beni hatırla özelliği

                // MVC sistemine "Bu kullanıcı giriş yaptı" diyoruz
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "E-posta veya şifre hatalı!");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterDto model)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonSerializer.Serialize(model);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var responseMessage = await client.PostAsync("https://localhost:7074/api/Users/register", stringContent);

            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Auth");
            }

            var errorContent = await responseMessage.Content.ReadAsStringAsync();

            try
            {
                using var jsonDoc = JsonDocument.Parse(errorContent);
                if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var error in jsonDoc.RootElement.EnumerateArray())
                    {
                        var description = error.GetProperty("description").GetString();
                        ModelState.AddModelError("", description);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Doğrulama Hatası: Lütfen tüm alanları formata uygun doldurun.");
                }
            }
            catch
            {
                ModelState.AddModelError("", "Kayıt işlemi başarısız oldu. Sistemsel bir hata oluştu.");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Tarayıcıdaki JWT çerezini sil
            Response.Cookies.Delete("JwtToken");

            return RedirectToAction("Index", "Home");
        }
    }
}
