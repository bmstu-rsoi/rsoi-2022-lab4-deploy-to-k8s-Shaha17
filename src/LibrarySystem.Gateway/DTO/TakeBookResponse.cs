namespace LibrarySystem.Gateway.DTO;

public class TakeBookResponse
{
    public Guid ReservationUid { get; set; }

    public string Status { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly TillDate { get; set; }

    public BookInfo Book { get; set; }

    public LibraryResponse Library { get; set; }

    public UserRatingResponse Rating { get; set; }
}