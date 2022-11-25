namespace LibrarySystem.Gateway.DTO;

public class BookInfo
{
    public Guid BookUid { get; set; }
    public string Name { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
}