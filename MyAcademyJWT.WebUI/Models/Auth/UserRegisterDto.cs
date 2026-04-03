namespace MyAcademyJWT.WebUI.Models.Auth
{
    public class UserRegisterDto
    {
        public string FullName { get; set; }
        public string? ImageUrl { get; set; }
        public int PackageId { get; set; } 
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
