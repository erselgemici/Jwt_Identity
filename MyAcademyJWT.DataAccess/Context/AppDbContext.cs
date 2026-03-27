using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyAcademyJWT.Entity.Entities;

namespace MyAcademyJWT.DataAccess.Context
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser, AppRole, int>(options)
    {
        public DbSet<Package> Packages { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<UserSongHistory> UserSongHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Package>().HasData(
                new Package { Id = 1, Name = "Elite", ContentLevel = 1 },
                new Package { Id = 2, Name = "Premium", ContentLevel = 2 },
                new Package { Id = 3, Name = "Gold", ContentLevel = 3 },
                new Package { Id = 4, Name = "Standard", ContentLevel = 4 },
                new Package { Id = 5, Name = "Basic", ContentLevel = 5 },
                new Package { Id = 6, Name = "Free", ContentLevel = 6 }
            );
        }
    }
}
