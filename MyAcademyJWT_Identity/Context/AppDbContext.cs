using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyAcademyJWT_Identity.Entities;

namespace MyAcademyJWT_Identity.Context
{
    public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, int>(options)
    {
        public DbSet<Category> Categories { get; set; }
    }
}
