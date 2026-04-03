namespace MyAcademyJWT.Business.DTOs.SongDtos
{
    public class SongPlayDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? AudioUrl { get; set; }
        public int RequiredContentLevel { get; set; }
    }
}
