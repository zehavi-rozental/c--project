using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    namespace BO;

    public class ProductInOrder
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }

        public double BasePrice { get; set; }

        public int Amount { get; set; }
        public IEnumerable<SaleInProduct>? Sales { get; set; }
        public double FinalPrice { get; set; }
        //public override string ToString() => this.ToStringProperty();
    }

