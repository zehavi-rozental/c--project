namespace DO;

    public record Customer
    (
         int Id ,
         string? Name ,
         string? Address ,
         string? PhoneNumber)
{ 
    public Customer() : this(0, null, null, null) { }
}
