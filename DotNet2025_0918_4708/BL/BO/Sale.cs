using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    // דריסת ToString באמצעות ה-Tools שתבני בהמשך (באמצעות Reflection)
    //public override string ToString() => this.ToStringProperty();
}