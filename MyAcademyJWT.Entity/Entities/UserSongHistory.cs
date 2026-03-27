using System;

namespace MyAcademyJWT.Entity.Entities
{
    public class UserSongHistory
    {
        public int Id { get; set; }

        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public int SongId { get; set; }
        public Song Song { get; set; }

        public DateTime ListenedAt { get; set; }
    }
}
