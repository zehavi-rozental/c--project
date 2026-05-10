namespace BO;

public class Client
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public override string ToString() => this.ToStringProperty();
}

