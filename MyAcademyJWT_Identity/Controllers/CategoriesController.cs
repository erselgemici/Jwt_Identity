using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAcademyJWT.DataAccess.Context;

namespace MyAcademyJWT_Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="User")]
    public class CategoriesController(AppDbContext _context) : ControllerBase
    {
        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var categories = await _context.Categories.ToListAsync();
        //    return Ok(categories);
        //}
    }
}
