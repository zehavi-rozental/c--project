using System;

namespace DO
{
    public record Customer
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }

        public Customer(int id = 0, string? name = null, string? address = null, string? phoneNumber = null)
        {
            Id = id;
            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
        }
    }
}