using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibrarySystem.LibrarySystem.Models
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
