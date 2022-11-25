using System.Reflection.Metadata.Ecma335;
using LibrarySystem.Gateway.Models;

namespace LibrarySystem.Gateway.DTO;

public class BookReservationResponse
{
    public Guid ReservationUid { get; set; }
    public string Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime TillDate { get; set; }
    public Book Book { get; set; }
    public Library Library { get; set; }
}