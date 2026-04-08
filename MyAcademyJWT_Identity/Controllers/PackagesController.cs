using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyAcademyJWT.Business.Services.PackageServices;
using MyAcademyJWT.Entity.Entities;
using System.Security.Claims;

namespace MyAcademyJWT_Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackagesController : ControllerBase
    {
        private readonly IPackageService _packageService;
        private readonly UserManager<AppUser> _userManager;

        public PackagesController(IPackageService packageService, UserManager<AppUser> userManager)
        {
            _packageService = packageService;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPackages()
        {
            var packages = await _packageService.GetAllPackagesAsync();

            if (packages == null || !packages.Any())
            {
                return NotFound("Sistemde kayıtlı paket bulunamadı.");
            }

            return Ok(packages);
        }

        [HttpPost("buy/{id}")]
        [Authorize] 
        public async Task<IActionResult> BuyPackage(int id)
        {
            // Token'dan istek yapan kullanıcının ID'sini bul
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized("Kullanıcı kimliği doğrulanamadı.");

            // Veritabanından kullanıcıyı bul
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("Kullanıcı bulunamadı.");

            // Kullanıcının paketini güncelle ve kaydet
            user.PackageId = id;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { message = "Paket başarıyla güncellendi." });
            }

            return BadRequest("Paket güncellenirken bir hata oluştu.");
        }
    }
}
