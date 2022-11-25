using System;
using System.Collections.Generic;

namespace LibrarySystem.ReservationSystem.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public Guid ReservationUid { get; set; }
        public string Username { get; set; } = null!;
        public Guid BookUid { get; set; }
        public Guid LibraryUid { get; set; }
        public string Status { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime TillDate { get; set; }
    }
}