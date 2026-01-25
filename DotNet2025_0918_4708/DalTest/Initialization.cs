using System;
using DalApi;
using DO;
namespace DelTest;

public static class Initialization
{
    private static IDal? s_dal;

    // מקבל את מופע ה-DAL ומאתחל את הנתונים
    public static void Initialize(IDal dal)
    {
        s_dal = dal;
        createSales();
        createCustomers();
        createProducts();
    }

    private static void createProducts()
    {
        // יצירת מוצרי בדיקה
        s_dal.Product.Create(new Product(1, "Camera A", Category.CAMERAS, 199.99, 10));
        s_dal.Product.Create(new Product(2, "Lens B", Category.LENSES, 99.99, 20));
        s_dal.Product.Create(new Product(3, "Tripod C", Category.TRIPODS, 49.99, 15));
        s_dal.Product.Create(new Product(4, "Photographic Book", Category.PHTOGRAFIC, 19.99, 30));
        s_dal.Product.Create(new Product(5, "Camera D", Category.CAMERAS, 299.99, 8));
        s_dal.Product.Create(new Product(6, "Lens E", Category.LENSES, 129.99, 12));
    }

    private static void createCustomers()
    {
        // יצירת לקוחות בדיקה
        s_dal.Customer.Create(new Customer(1, "Rivki", "Meromei Sade", "123456789"));
        s_dal.Customer.Create(new Customer(2, "Gitty", "Ktsot", "1357925"));
        s_dal.Customer.Create(new Customer(3, "Yehudit", "Shaagat Arie", "431221111"));
        s_dal.Customer.Create(new Customer(4, "Tovi", "Mesilat Yosef", "464575678"));
        s_dal.Customer.Create(new Customer(5, "Shulamit", "Netivot Hamishpat", "78787878"));
        s_dal.Customer.Create(new Customer(6, "Dvory", "Rabi Akiva", "57453243"));
    }

    private static void createSales()
    {
        // יצירת מבצעי בדיקה
        s_dal.Sale.Create(new Sale(1, 1, 1, 199.99, true, new DateTime(2025, 6, 15), new DateTime(2025, 6, 25)));
        s_dal.Sale.Create(new Sale(2, 2, 2, 170.00, false, new DateTime(2025, 7, 1), new DateTime(2025, 7, 30)));
        s_dal.Sale.Create(new Sale(0, 3, 1, 40.00, true, new DateTime(2025, 8, 1), new DateTime(2025, 8, 10)));
        s_dal.Sale.Create(new Sale(0, 4, 3, 50.00, false, new DateTime(2025, 9, 1), new DateTime(2025, 9, 10)));
        s_dal.Sale.Create(new Sale(5, 5, 2, 500.00, true, new DateTime(2025, 10, 1), new DateTime(2025, 10, 15)));
    }
}