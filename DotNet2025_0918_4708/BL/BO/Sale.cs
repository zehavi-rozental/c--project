using System;

namespace BO;

public class Sale
{
    public int Id { get; init; }
    public int ProductId { get; set; }
    public int AmmontRequird { get; set; }
    public double TotalPrice { get; set; }
    public bool IsClubMembers { get; set; }
    public DateTime StartSale { get; set; }
    public DateTime EndSale { get; set; }

    public override string ToString() => this.ToStringProperty();
}
