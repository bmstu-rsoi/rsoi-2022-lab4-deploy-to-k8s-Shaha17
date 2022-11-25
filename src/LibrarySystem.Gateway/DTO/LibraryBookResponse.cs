namespace LibrarySystem.Gateway.DTO;

public class LibraryBookResponse
{
    public Guid BookUid { get; set; }
    public string Name { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public string Condition { get; set; }
    public int AvaiblableCount { get; set; }
}