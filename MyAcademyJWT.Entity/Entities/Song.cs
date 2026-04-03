using System;
using System.Collections.Generic;

namespace MyAcademyJWT.Entity.Entities
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public long DeezerTrackId { get; set; }
        public TimeSpan Duration { get; set; } 

        public int RequiredContentLevel { get; set; }

        public int AlbumId { get; set; }
        public Album Album { get; set; }

        public ICollection<UserSongHistory> UserSongHistories { get; set; }
    }
}
