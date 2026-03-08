using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO;

public class Order
{
    public int Id { get; init; }
    public bool IsPreferredCustomer { get; set; }
    public IEnumerable<ProductInOrder>? Items { get; set; }

    public double TotalPrice { get; set; }

    public DateTime? OrderDate { get; set; }

    //public override string ToString() => this.ToStringProperty();
}
