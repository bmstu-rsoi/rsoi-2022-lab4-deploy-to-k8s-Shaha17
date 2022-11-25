using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.LibrarySystem.Models
{
    public class LibraryBook
    {
        public int? BookId { get; set; }
        public int? LibraryId { get; set; }
        public int AvailableCount { get; set; }

        public virtual Book? Book { get; set; }
        public virtual Library? Library { get; set; }
    }
}
