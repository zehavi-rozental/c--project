using System;
using Dal;
using DalApi;
using DO;

namespace DelTest;

class Program
{
    private static IDal s_dal = new Dal.DalList();

    static void Main()
    {
        try
        {
            Initialization.Initialize(s_dal);
        }
        catch (IdNotFoundException e)
        {
            Console.WriteLine($"Caught an exception: {e.Message}");
        }
        catch (IdAlreadyExistsException e)
        {
            Console.WriteLine($"Caught an exception: {e.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Caught an exception: {e.Message}");
        }

    }

    private static int ShowMainMenu()

    {
        Console.WriteLine();
        Console.WriteLine("Main menu:");
        Console.WriteLine("1. Customer");
        Console.WriteLine("2. Product");
        Console.WriteLine("3. Sale");
        Console.WriteLine("4. Exit");
        Console.Write("Select entity: ");
        var s = Console.ReadLine();
        if (int.TryParse(s, out int c)) return c;
        return -1;
    }

    private static void CrudEntity(string entity)
    {
        bool back = false;
        while (!back)
        {
            int choice = ShowCrudMenu(entity);
            switch (choice)
            {
                case 1: Create(entity); break;
                case 2:
                    if (entity == "Customer") Read(s_dal.customer);
                    else if (entity == "Product") Read(s_dal.product);
                    else if (entity == "Sale") Read(s_dal.sale);
                    break;
                case 3:
                    if (entity == "Customer") ReadAll(s_dal.customer);
                    else if (entity == "Product") ReadAll(s_dal.product);
                    else if (entity == "Sale") ReadAll(s_dal.sale);
                    break;
                case 4: Update(entity); break;
                case 5:
                    if (entity == "Customer") Delete(s_dal.customer);
                    else if (entity == "Product") Delete(s_dal.product);
                    else if (entity == "Sale") Delete(s_dal.sale);
                    break;
                case 6: back = true; break;
                default: Console.WriteLine("Unknown choice."); break;
            }
        }
    }

    private static int ShowCrudMenu(string entity)
    {
        Console.WriteLine();
        Console.WriteLine($"CRUD menu for {entity}:");
        Console.WriteLine("1. Create");
        Console.WriteLine("2. Read by id");
        Console.WriteLine("3. Read all");
        Console.WriteLine("4. Update");
        Console.WriteLine("5. Delete");
        Console.WriteLine("6. Back");
        Console.Write("Select operation: ");
        var s = Console.ReadLine();
        if (int.TryParse(s, out int c)) return c;
        return -1;
    }

    private static void Create(string entity)
    {
        try
        {
            if (entity == "Customer") CreateCustomer();
            else if (entity == "Product") CreateProduct();
            else if (entity == "Sale") CreateSale();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Create failed: " + ex);
        }
    }

    private static void CreateCustomer()
    {
        Console.Write("CustomerId (int): "); int.TryParse(Console.ReadLine(), out int id);
        Console.Write("Name: "); string name = Console.ReadLine() ?? "";
        Console.Write("Address: "); string addr = Console.ReadLine() ?? "";
        Console.Write("Phone: "); string phone = Console.ReadLine() ?? "";
        var c = new Customer(id, name, addr, phone);
        int newId = s_dal.customer.Create(c);
        Console.WriteLine($"Created customer id: {newId}");
    }

    private static void CreateProduct()
    {
        //Console.Write("ProductId (int): "); int.TryParse(Console.ReadLine(), out int id);
        Console.Write("Name: "); string name = Console.ReadLine() ?? "";
        Console.Write("Category (int): "); int.TryParse(Console.ReadLine(), out int cat);
        Console.Write("Price (double): "); double.TryParse(Console.ReadLine(), out double price);
        Console.Write("Count (int): "); int.TryParse(Console.ReadLine(), out int count);
        var p = new Product(0, name, (ProductsCategories)cat, price, count);
        int newId = s_dal.product.Create(p);
        Console.WriteLine($"Created product id: {newId}");
    }

    private static void CreateSale()
    {
        //Console.Write("SaleId (int): "); int.TryParse(Console.ReadLine(), out int id);
        Console.Write("ProductId (int): "); int.TryParse(Console.ReadLine(), out int pid);
        Console.Write("ProductsCountToSale (int): "); int.TryParse(Console.ReadLine(), out int cnt);
        Console.Write("PriceAfterSale (int): "); int.TryParse(Console.ReadLine(), out int price);
        Console.Write("OnlyClubCustomers (true/false): "); bool.TryParse(Console.ReadLine(), out bool only);
        Console.Write("DateStart (yyyy-MM-dd) or empty: "); string ds = Console.ReadLine() ?? "";
        DateTime? dateStart = null; if (DateTime.TryParse(ds, out var dt1)) dateStart = dt1;
        Console.Write("DateEnd (yyyy-MM-dd) or empty: "); string de = Console.ReadLine() ?? "";
        DateTime? dateEnd = null; if (DateTime.TryParse(de, out var dt2)) dateEnd = dt2;
        var s = new Sale(0, pid, cnt, price, only, dateStart, dateEnd);
        int newId = s_dal.sale.Create(s);
        Console.WriteLine($"Created sale id: {newId}");
    }

    private static void Read<T>(Icrud<T> repo)
    {
        try
        {
            Console.Write("Id: "); int.TryParse(Console.ReadLine(), out int id);
            var item = repo.Read(id);
            Console.WriteLine(item == null ? "null" : item.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine("Read failed: " + ex);
        }
    }

    private static void ReadAll<T>(Icrud<T> repo)
    {
        try
        {
            var list = repo.ReadAll();
            foreach (var it in list) Console.WriteLine(it);
        }
        catch (Exception ex)
        {
            Console.WriteLine("ReadAll failed: " + ex);
        }
    }

    private static void Update(string entity)
    {
        try
        {
            if (entity == "Customer")
            {
                Console.Write("CustomerId (existing id): "); int.TryParse(Console.ReadLine(), out int id);
                Console.Write("Name: "); string name = Console.ReadLine() ?? "";
                Console.Write("Address: "); string addr = Console.ReadLine() ?? "";
                Console.Write("Phone: "); string phone = Console.ReadLine() ?? "";
                var c = new Customer(id, name, addr, phone);
                s_dal.customer.Update(c);
                Console.WriteLine("Updated.");
            }
            else if (entity == "Product")
            {
                Console.Write("ProductId: "); int.TryParse(Console.ReadLine(), out int id);
                Console.Write("Name: "); string name = Console.ReadLine() ?? "";
                Console.Write("Category (int): "); int.TryParse(Console.ReadLine(), out int cat);
                Console.Write("Price (double): "); double.TryParse(Console.ReadLine(), out double price);
                Console.Write("Count (int): "); int.TryParse(Console.ReadLine(), out int count);
                var p = new Product(id, name, (ProductsCategories)cat, price, count);
                s_dal.product.Update(p);
                Console.WriteLine("Updated.");
            }
            else if (entity == "Sale")
            {
                Console.Write("SaleId: "); int.TryParse(Console.ReadLine(), out int id);
                Console.Write("ProductId: "); int.TryParse(Console.ReadLine(), out int pid);
                Console.Write("ProductsCountToSale: "); int.TryParse(Console.ReadLine(), out int cnt);
                Console.Write("PriceAfterSale: "); int.TryParse(Console.ReadLine(), out int price);
                Console.Write("OnlyClubCustomers (true/false): "); bool.TryParse(Console.ReadLine(), out bool only);
                Console.Write("DateStart (yyyy-MM-dd) or empty: "); string ds = Console.ReadLine() ?? "";
                DateTime? dateStart = null; if (DateTime.TryParse(ds, out var dt1)) dateStart = dt1;
                Console.Write("DateEnd (yyyy-MM-dd) or empty: "); string de = Console.ReadLine() ?? "";
                DateTime? dateEnd = null; if (DateTime.TryParse(de, out var dt2)) dateEnd = dt2;
                var s = new Sale(id, pid, cnt, price, only, dateStart, dateEnd);
                s_dal.sale.Update(s);
                Console.WriteLine("Updated.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Update failed: " + ex);
        }
    }

    private static void Delete<T>(Icrud<T> repo)
    {
        try
        {
            Console.Write("Id: "); int.TryParse(Console.ReadLine(), out int id);
            repo.Delete(id);
            Console.WriteLine("Deleted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Delete failed: " + ex);
        }
    }
}