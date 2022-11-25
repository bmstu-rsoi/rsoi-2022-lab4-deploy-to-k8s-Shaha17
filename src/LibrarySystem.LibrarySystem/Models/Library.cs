using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibrarySystem.LibrarySystem.Models
{
    public  class Library
    {
        public int Id { get; set; }
        public Guid LibraryUid { get; set; }
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Address { get; set; } = null!;
        

    }
}
