namespace MyAcademyJWT.Entity.Entities
{
    public class Album
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public string CoverImageUrl { get; set; }
        public int ArtistId { get; set; }
        public Artist Artist { get; set; }
        public ICollection<Song> Songs { get; set; }
    }
}
