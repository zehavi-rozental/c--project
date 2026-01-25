namespace DO;

public record Customer(int Id, string? Name, string? Address, string? PhoneNumber)
{
    public Customer() : this(1, "Rivki", "Meromei Sade", "123456789") { }
}