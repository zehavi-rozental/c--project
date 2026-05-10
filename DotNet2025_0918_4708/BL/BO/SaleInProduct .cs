namespace BO;

public class SaleInProduct
{
    public int Id { get; init; }
    public int AmountRequired { get; set; }
    public double SalePrice { get; set; }
    public bool IsForEveryone { get; set; }

    public override string ToString() => this.ToStringProperty();
}

