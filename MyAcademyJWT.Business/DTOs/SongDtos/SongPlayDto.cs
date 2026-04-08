namespace MyAcademyJWT.Business.DTOs.SongDtos
{
    public class SongPlayDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? AudioUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? ArtistName { get; set; }
        public int RequiredContentLevel { get; set; }
    }
}
