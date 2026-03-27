namespace MyAcademyJWT.Entity.Entities
{
    public class Package
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public int ContentLevel { get; set; } 
        public ICollection<AppUser> Users { get; set; }
    }
}
