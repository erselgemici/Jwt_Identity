using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyAcademyJWT.Business.DTOs.UserDtos;
using MyAcademyJWT.Business.Services.JwtServices;
using MyAcademyJWT.Entity.Entities;

namespace MyAcademyJWT_Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(UserManager<AppUser> _userManager,SignInManager<AppUser> _signInManager,
        IJwtService _jwtService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var user = new AppUser
            {
                FullName = model.FullName,        
                UserName = model.UserName,
                Email = model.Email,
                ImageUrl = model.ImageUrl,       

                PackageId = model.PackageId > 0 ? model.PackageId : 1
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Kullanıcı Kaydı Başarılı");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {



            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

            if (!result.Succeeded)
            {
                return BadRequest("Kullanıcı adı veya şifre hatalı");
            }

            var user = await _userManager.FindByNameAsync(model.UserName);

            var token = await _jwtService.GenerateTokenAsync(user);

            return Ok(new {token = token});
        }

    }

    
}
