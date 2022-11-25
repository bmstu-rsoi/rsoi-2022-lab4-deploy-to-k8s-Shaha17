namespace LibrarySystem.Gateway.DTO;

public class TakeBookRequest
{
    public Guid BookUid { get; set; }
    public Guid LibraryUid { get; set; }
    public DateTime TillDate { get; set; }
}