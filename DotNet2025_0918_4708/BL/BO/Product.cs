namespace BO;

public class Product
{
    public int Id { get; init; }
    public string ProductName { get; set; } = string.Empty;
    public Category Category { get; set; }
    public double Price { get; set; }
    public int Ammount { get; set; }
    public IEnumerable<SaleInProduct>? Sales { get; set; }

    public override string ToString() => this.ToStringProperty();
}
