using Microsoft.AspNetCore.Identity;

namespace MyAcademyJWT.Entity.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public string FullName { get; set; }
        public string? ImageUrl { get; set; }
        public int PackageId { get; set; }
        public Package Package { get; set; } 
        public ICollection<UserSongHistory> SongHistories { get; set; }
    }
}
