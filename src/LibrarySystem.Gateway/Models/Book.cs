namespace LibrarySystem.Gateway.Models
{
    public class Book
    {
        public int Id { get; set; }
        public Guid BookUid { get; set; }
        public string Name { get; set; } = null!;
        public string? Author { get; set; }
        public string? Genre { get; set; }
        public string? Condition { get; set; }
        
    }
}
