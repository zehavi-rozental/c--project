using System;
using System.Collections.Generic;

namespace BO;

public class Order
{
    public int Id { get; init; }
    public bool IsPreferredCustomer { get; set; }
    public List<ProductInOrder> Items { get; set; } = new();
    public double TotalPrice { get; set; }
    public DateTime? OrderDate { get; set; }

    public override string ToString() => this.ToStringProperty();
}
