namespace MyAcademyJWT.WebUI.Models.Song
{
    public class ResultSongDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public string AlbumTitle { get; set; }
        public string CoverImageUrl { get; set; } 
        public string Duration { get; set; }
        public int RequiredContentLevel { get; set; }
    }

    
}
