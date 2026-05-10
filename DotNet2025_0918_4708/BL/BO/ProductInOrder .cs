using System;
using System.Collections.Generic;

namespace BO;

public class ProductInOrder
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public double BasePrice { get; set; }
    public int Amount { get; set; }
    public List<SaleInProduct> Sales { get; set; } = new();
    public double FinalPrice { get; set; }

    public override string ToString() => this.ToStringProperty();
}

