using System;
using Dal;
using DalApi;
using DO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace DelTest;
class Program
{
    // הגדרת משתנה סטטי לממשק הגישה לנתונים, הממומש על ידי DalList

    private static IDal s_dal = new DalList();
    static void Main()
    {

        try
        {
            // אתחול הנתונים הראשוניים
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

        int choice = -1;
        // לולאה ראשית להצגת התפריט
        while ((choice = ShowMainMenu()) != 4)
        {
            switch (choice)
            {
                case 1:
                    CrudEntity("Customer");
                    break;
                case 2:
                    CrudEntity("Product");
                    break;
                case 3:
                    CrudEntity("Sale");
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
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

    // הפעולה CrudEntity: מציגה תפריט CRUD לישות שנבחרה ומפעילה את המתודות המתאימות
    private static void CrudEntity(string entity)
    {
        bool back = false;
        while (!back)
        {
            int choice = ShowCrudMenu(entity);
            try
            {
                switch (choice)
                {
                    case 1: // Create
                        switch (entity)
                        {
                            case "Customer": Create(s_dal.Customer); break;
                            case "Product": Create(s_dal.Product); break;
                            case "Sale": Create(s_dal.Sale); break;
                        }
                        break;
                    case 2: // Read by ID
                        switch (entity)
                        {
                            case "Customer": ReadById(s_dal.Customer); break;
                            case "Product": ReadById(s_dal.Product); break;
                            case "Sale": ReadById(s_dal.Sale); break;
                        }
                        break;
                    case 3: // Read all
                        switch (entity)
                        {
                            case "Customer": ReadAll(s_dal.Customer); break;
                            case "Product": ReadAll(s_dal.Product); break;
                            case "Sale": ReadAll(s_dal.Sale); break;
                        }
                        break;
                    case 4: // Update
                        switch (entity)
                        {
                            case "Customer": Update(s_dal.Customer); break;
                            case "Product": Update(s_dal.Product); break;
                            case "Sale": Update(s_dal.Sale); break;
                        }
                        break;
                    case 5: // Delete
                        switch (entity)
                        {
                            case "Customer": Delete(s_dal.Customer); break;
                            case "Product": Delete(s_dal.Product); break;
                            case "Sale": Delete(s_dal.Sale); break;
                        }
                        break;
                    case 6: // Back
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Operation failed: " + ex.Message);
            }
        }
    }

    private static int ShowCrudMenu(string entity)
    {
        Console.WriteLine();
        Console.WriteLine($"CRUD menu for {entity}:");
        Console.WriteLine("1. Create");
        Console.WriteLine("2. Read by ID");
        Console.WriteLine("3. Read all");
        Console.WriteLine("4. Update");
        Console.WriteLine("5. Delete");
        Console.WriteLine("6. Back to main menu");
        Console.Write("Select operation: ");
        var s = Console.ReadLine();
        if (int.TryParse(s, out int c)) return c;
        return -1;
    }

    // מתודת Create גנרית
    private static void Create<T>(ICrud<T> repo)
    {
        try
        {
            if (typeof(T) == typeof(Customer))
            {
                Console.Write("Id: "); int.TryParse(Console.ReadLine(), out int id);
                Console.Write("Name: "); string name = Console.ReadLine() ?? "";
                Console.Write("Address: "); string address = Console.ReadLine() ?? "";
                Console.Write("Phone: "); string phone = Console.ReadLine() ?? "";
                var c = new Customer(id, name, address, phone);
                int newId = repo.Create((T)(Object)c);
                Console.WriteLine($"Created: {c} with ID: {newId}");
            }
            else if (typeof(T) == typeof(Product))
            {
                Console.Write("Id: "); int.TryParse(Console.ReadLine(), out int id);
                Console.Write("ProductName: "); string name = Console.ReadLine() ?? "";
                Console.Write("Category (CAMERAS/LENSES/TRIPODS/PHTOGRAFIC): "); Category category = Enum.Parse<Category>(Console.ReadLine() ?? "");
                Console.Write("Price: "); double.TryParse(Console.ReadLine(), out double price);
                Console.Write("Amount: "); int.TryParse(Console.ReadLine(), out int amount);
                var p = new Product(id, name, category, price, amount);
                int newId = repo.Create((T)(object)p);
                Console.WriteLine($"Created: {p} with ID: {newId}");
            }
            else if (typeof(T) == typeof(Sale))
            {
                Console.Write("Id: "); int.TryParse(Console.ReadLine(), out int id);
                Console.Write("ProductId: "); int.TryParse(Console.ReadLine(), out int pid);
                Console.Write("ProductsCountToSale: "); int.TryParse(Console.ReadLine(), out int cnt);
                Console.Write("PriceAfterSale: "); double.TryParse(Console.ReadLine(), out double price);
                Console.Write("OnlyClubCustomers (true/false): "); bool.TryParse(Console.ReadLine(), out bool only);

                Console.Write("DateStart (yyyy-MM-dd): ");
                DateTime dateStart = DateTime.TryParse(Console.ReadLine(), out var dt1) ? dt1 : DateTime.Now; // אם הקלט שגוי, משתמשים בזמן הנוכחי

                Console.Write("DateEnd (yyyy-MM-dd): ");
                DateTime dateEnd = DateTime.TryParse(Console.ReadLine(), out var dt2) ? dt2 : DateTime.Now.AddDays(7); // אם הקלט שגוי, משתמשים בשבוע מהיום

                var s = new Sale(id, pid, cnt, price, only, dateStart, dateEnd);
                int newId = repo.Create((T)(object)s);
                Console.WriteLine($"Created: {s} with ID: {newId}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Creation failed: " + ex.Message);
        }
    }

    // מתודת ReadById גנרית
    private static void ReadById<T>(ICrud<T> repo)
    {
        try
        {
            Console.Write("Id: "); int.TryParse(Console.ReadLine(), out int id);
            T? item = repo.Read(id);
            if (item != null)
                Console.WriteLine($"Read: {item}");
            else
                Console.WriteLine("Item not found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Read failed: " + ex.Message);
        }
    }

    // מתודת ReadAll גנרית
    private static void ReadAll<T>(ICrud<T> repo)
    {
        try
        {
            List<T> all = repo.ReadAll();
            if (all.Any())
            {
                Console.WriteLine($"All {typeof(T).Name}s:");
                foreach (var item in all)
                {
                    Console.WriteLine(item);
                }
            }
            else
            {
                Console.WriteLine($"No {typeof(T).Name}s found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Read all failed: " + ex.Message);
        }
    }

    // מתודת Update גנרית
    private static void Update<T>(ICrud<T> repo)
    {
        try
        {
            Console.Write("Id to update: "); int.TryParse(Console.ReadLine(), out int id);

            if (typeof(T) == typeof(Customer))
            {
                Console.Write("Name: "); string name = Console.ReadLine() ?? "";
                Console.Write("Address: "); string address = Console.ReadLine() ?? "";
                Console.Write("Phone: "); string phone = Console.ReadLine() ?? "";
                var c = new Customer(id, name, address, phone);
                repo.Update((T)(object)c);
                Console.WriteLine("Updated.");
            }
            else if (typeof(T) == typeof(Product))
            {
                Console.Write("ProductName: "); string name = Console.ReadLine() ?? "";
                Console.Write("Category (CAMERAS/LENSES/TRIPODS/PHTOGRAFIC): "); Category category = Enum.Parse<Category>(Console.ReadLine() ?? "");
                Console.Write("Price: "); double.TryParse(Console.ReadLine(), out double price);
                Console.Write("Amount: "); int.TryParse(Console.ReadLine(), out int amount);
                var p = new Product(id, name, category, price, amount);
                repo.Update((T)(object)p);
                Console.WriteLine("Updated.");
            }
            else if (typeof(T) == typeof(Sale))
            {
                Console.Write("ProductId: "); int.TryParse(Console.ReadLine(), out int pid);
                Console.Write("ProductsCountToSale: "); int.TryParse(Console.ReadLine(), out int cnt);
                Console.Write("PriceAfterSale: "); double.TryParse(Console.ReadLine(), out double price);
                Console.Write("OnlyClubCustomers (true/false): "); bool.TryParse(Console.ReadLine(), out bool only);
                Console.Write("DateStart (yyyy-MM-dd): "); DateTime.TryParse(Console.ReadLine(), out DateTime dt1);
                Console.Write("DateEnd (yyyy-MM-dd): "); DateTime.TryParse(Console.ReadLine(), out DateTime dt2);
                var s = new Sale(id, pid, cnt, price, only, dt1, dt2);
                repo.Update((T)(object)s);


                Console.WriteLine("Updated.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Update failed: " + ex.Message);
        }
    }

    // מתודת Delete גנרית
    private static void Delete<T>(ICrud<T> repo)
    {
        try
        {
            Console.Write("Id: "); int.TryParse(Console.ReadLine(), out int id);
            repo.Delete(id);
            Console.WriteLine("Deleted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Delete failed: " + ex.Message);
        }
    }
}