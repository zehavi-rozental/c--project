using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace BO;

    public class Product
    {
        // תכונות זהות ל-DO.Product (עם התאמה לכללי ה-BO)
        public int Id { get; init; } // מזהה ייחודי, לרוב לקריאה בלבד לאחר האתחול
        public string? ProductName { get; set; }
        public Category? Category { get; set; } // שימי לב שזה ה-Category של BO
        public double Price { get; set; }
        public int Ammount { get; set; }

        // התכונה הנוספת שנדרשה: רשימת מבצעים למוצר
        public IEnumerable<SaleInProduct>? Sales { get; set; }

        // דריסת ToString באמצעות ה-Tools (שבנית או תבני בהמשך)
        //public override string ToString() => this.ToStringProperty();
    
}
