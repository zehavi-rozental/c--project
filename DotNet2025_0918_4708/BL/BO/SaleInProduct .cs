using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    namespace BO;

    public class SaleInProduct
    {
        public int Id { get; init; }

        public int AmountRequired { get; set; }

        // מחיר (double) - המחיר המבצע ליחידה או לכל הכמות
        public double SalePrice { get; set; }

        public bool IsForEveryone { get; set; }

        // דריסת ToString באמצעות ה-Tools (Reflection)
       // public override string ToString() => this.ToStringProperty();
    }

