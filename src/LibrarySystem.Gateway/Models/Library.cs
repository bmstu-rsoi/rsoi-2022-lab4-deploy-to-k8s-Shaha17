namespace LibrarySystem.Gateway.Models
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
