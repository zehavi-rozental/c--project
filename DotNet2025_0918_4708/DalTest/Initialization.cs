using System;
using DalApi;
using DO;
namespace DelTest;
public static class Initialization
{
    private static IDal? s_dal;

    public static void Initialize(IDal dal)
    {
        s_dal = dal;
        createSales();
        createCustomers();
        createProducts();
    }

    private static void createProducts()
    {
      
       s_dal.Product.Create(new Product(1, "Camera A", Category.CAMERAS, 199.99, 10));
        s_dal.Product.Create(new Product(2, "Lens B", Category.LENSES, 99.99, 20));
        s_dal.Product.Create(new Product(3, "Tripod C", Category.TRIPODS, 49.99, 15));
        s_dal.Product.Create(new Product(4, "Photographic Book", Category.PHTOGRAFIC, 19.99, 30));
        s_dal.Product.Create(new Product(5, "Camera D", Category.CAMERAS, 299.99, 8));
        s_dal.Product.Create(new Product(6, "Lens E", Category.LENSES, 129.99, 12));
    }

    private static void createCustomers()
    {
      
        s_dal.Customer.Create(new Customer(1, "Rivki", "Meromei Sade", "123456789"));
        s_dal.Customer.Create(new Customer(2, "Gitty", "Ktsot", "1357925"));
        s_dal.Customer.Create(new Customer(3, "Yehudit", "Shaagat Arie", "431221111"));
        s_dal.Customer.Create(new Customer(4, "Tovi", "Mesilat Yosef", "464575678"));
        s_dal.Customer.Create(new Customer(5, "Shulamit", "Netivot Hamishpat", "78787878"));
        s_dal.Customer.Create(new Customer(6, "Dvory", "Rabi Akiva", "57453243"));
    }

    private static void createSales()
    {
        
        s_dal.Sale.Create(new Sale(1, 1, 1, 199.99, true, new DateTime(2025, 6, 15), new DateTime(2025, 6, 20)));
        s_dal.Sale.Create(new Sale(2, 2, 1, 99.99, false, new DateTime(2025, 6, 16), new DateTime(2025, 6, 21)));
        s_dal.Sale.Create(new Sale(3, 3, 2, 49.99, true, new DateTime(2025, 6, 17), new DateTime(2025, 6, 22)));
        s_dal.Sale.Create(new Sale(4, 4, 5, 19.99, true, new DateTime(2025, 6, 18), new DateTime(2025, 6, 23)));
        s_dal.Sale.Create(new Sale(5, 5, 3, 299.99, false, new DateTime(2025, 6, 19), new DateTime(2025, 6, 24)));
        s_dal.Sale.Create(new Sale(6, 6, 4, 129.99, true, new DateTime(2025, 6, 20), new DateTime(2025, 6, 25)));
    }
}
