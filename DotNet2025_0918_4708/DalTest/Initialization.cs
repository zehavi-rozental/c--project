using System;
using DalApi;
using DO;
namespace DelTest;

public static class Initialization
{
    private static IDal? s_dal;

    // מקבל את מופע ה-DAL ומאתחל את הנתונים
    public static void Initialize()
    {

        s_dal = DalApi.Factory.Get;
        createSales();
        createCustomers();
        createProducts();
    }

    private static void createProducts()
    {
        // יצירת מוצרי בדיקה
        s_dal!.Product.Create(new Product(1, "Smart Ambient Bulb", Category.LIGHTING, 79.99, 25));
        s_dal!.Product.Create(new Product(2, "Connected Security Camera", Category.SECURITY, 199.99, 12));
        s_dal!.Product.Create(new Product(3, "Smart Thermostat", Category.CLIMATE, 249.99, 10));
        s_dal!.Product.Create(new Product(4, "Voice Assistant Hub", Category.AUDIO, 129.99, 18));
        s_dal!.Product.Create(new Product(5, "Outdoor Floodlight", Category.LIGHTING, 159.99, 8));
        s_dal!.Product.Create(new Product(6, "Smart Door Lock", Category.SECURITY, 289.99, 6));
    }
    private static void createCustomers()
     {  

// הוספת קופאי/לקוח רגיל
        // יצירת לקוחות בדיקה
        s_dal!.Customer.Create(new DO.Customer(1, "Manager", "admin", "123", "Main St", "0501234567", true));
        s_dal!.Customer.Create(new DO.Customer(2, "Cashier", "user", "123", "Second St", "0507654321", false));
     }
    private static void createSales()
{
    var now = DateTime.Now;

    // מוצר 1 (Smart Ambient Bulb) — שני מבצעים במקביל
    // מבצע A: קנה 2 ב-120 ₪ (60 ₪ ליחידה)
    s_dal!.Sale.Create(new Sale(1, 1, 2, 120.00, false, now.AddDays(-1), now.AddDays(30)));
    // מבצע B: קנה 5 ב-250 ₪ (50 ₪ ליחידה) — משתלם יותר בכמות גדולה
    s_dal!.Sale.Create(new Sale(2, 1, 5, 250.00, false, now.AddDays(-1), now.AddDays(30)));
    // מבצע C: קנה 3 ב-165 ₪ (55 ₪ ליחידה) — לחברי מועדון בלבד
    s_dal!.Sale.Create(new Sale(3, 1, 3, 165.00, true, now.AddDays(-1), now.AddDays(30)));

    // מוצר 2 (Connected Security Camera) — שני מבצעים
    // מבצע D: קנה 2 ב-350 ₪ (175 ₪ ליחידה)
    s_dal!.Sale.Create(new Sale(4, 2, 2, 350.00, false, now.AddDays(-1), now.AddDays(30)));
    // מבצע E: קנה 4 ב-640 ₪ (160 ₪ ליחידה) — משתלם יותר
    s_dal!.Sale.Create(new Sale(5, 2, 4, 640.00, false, now.AddDays(-1), now.AddDays(30)));

    // מוצר 3 — מבצע יחיד
    s_dal!.Sale.Create(new Sale(6, 3, 1, 200.00, false, now.AddDays(-1), now.AddDays(30)));
}
}