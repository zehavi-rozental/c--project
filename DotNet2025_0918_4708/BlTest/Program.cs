using BlApi;
using BO;

static class Program
{
    private static readonly IBl s_bl = Factory.Get();
    private static Order? s_currentOrder;

    public static void Main()
    {
        try
        {
            DelTest.Initialization.Initialize();
            Console.WriteLine("DAL test data initialized.\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize DAL test data: {ex.Message}");
        }

        while (true)
        {
            PrintMenu();
            var input = Console.ReadLine()?.Trim();
            Console.WriteLine();

            switch (input)
            {
                case "L":
                case "l":
                    TestLogin();
                    break;
                case "1": ListCustomers(); break;
                case "2": AddCustomer(); break;
                case "3": ListProducts(); break;
                case "4": AddProduct(); break;
                case "5": ListSales(); break;
                case "6": AddSale(); break;
                case "7": CreateOrder(); break;
                case "8": AddProductToCurrentOrder(); break;
                case "9": ShowCurrentOrder(); break;
                case "10": FinalizeOrder(); break;
                case "0": return;
                default:
                    Console.WriteLine("Invalid selection. Try again.");
                    break;
            }

            Console.WriteLine();
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine("BL manual test menu:");
        Console.WriteLine("L - Test login");
        Console.WriteLine("1 - List customers");
        Console.WriteLine("2 - Add customer");
        Console.WriteLine("3 - List products");
        Console.WriteLine("4 - Add product");
        Console.WriteLine("5 - List sales");
        Console.WriteLine("6 - Add sale");
        Console.WriteLine("7 - Create new order");
        Console.WriteLine("8 - Add product to current order");
        Console.WriteLine("9 - Show current order");
        Console.WriteLine("10 - Finalize current order");
        Console.WriteLine("0 - Exit");
        Console.Write("Choose an option: ");
    }

    private static void ListCustomers()
    {
        Console.WriteLine("Customers:");
        foreach (var customer in s_bl.Client.ReadAll())
            Console.WriteLine(customer.ToString());
    }

    private static void AddCustomer()
    {
        Console.WriteLine("Create new customer:");
        var id = ReadInt("Customer id: ");
        var name = ReadString("Name: ");
        var address = ReadString("Address: ");
        var phone = ReadString("Phone number: ");

        try
        {
            s_bl.Client.Create(new Client { Id = id, Name = name, Address = address, PhoneNumber = phone });
            Console.WriteLine("Customer created.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create customer: {ex.Message}");
        }
    }

    private static void ListProducts()
    {
        Console.WriteLine("Products:");
        foreach (var product in s_bl.Product.ReadAll())
            Console.WriteLine(product.ToString());
    }

    private static void AddProduct()
    {
        Console.WriteLine("Create new product:");
        var id = ReadInt("Product id: ");
        var name = ReadString("Name: ");
        var category = ReadCategory("Category (CAMERAS, LENSES, TRIPODS, PHTOGRAFIC): ");
        var price = ReadDouble("Price: ");
        var amount = ReadInt("Amount: ");

        try
        {
            s_bl.Product.Create(new Product { Id = id, ProductName = name, Category = category, Price = price, Ammount = amount });
            Console.WriteLine("Product created.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create product: {ex.Message}");
        }
    }

    private static void ListSales()
    {
        Console.WriteLine("Sales:");
        foreach (var sale in s_bl.Sale.ReadAll())
            Console.WriteLine(sale.ToString());
    }

    private static void AddSale()
    {
        Console.WriteLine("Create new sale:");
        var id = ReadInt("Sale id: ");
        var productId = ReadInt("Product id: ");
        var amountRequired = ReadInt("Amount required: ");
        var price = ReadDouble("Sale price: ");
        var isClub = ReadBool("Is this sale for club members only? (y/n): ");
        var startDate = ReadDate("Start date (yyyy-MM-dd): ");
        var endDate = ReadDate("End date (yyyy-MM-dd): ");

        try
        {
            s_bl.Sale.Create(new Sale
            {
                Id = id,
                ProductId = productId,
                AmmontRequird = amountRequired,
                TotalPrice = price,
                IsClubMembers = isClub,
                StartSale = startDate,
                EndSale = endDate
            });
            Console.WriteLine("Sale created.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create sale: {ex.Message}");
        }
    }

    private static void CreateOrder()
    {
        var id = ReadInt("Order id: ");
        var preferred = ReadBool("Is the customer preferred? (y/n): ");

        s_currentOrder = new Order { Id = id, IsPreferredCustomer = preferred };
        Console.WriteLine("New order created.");
    }

    private static void AddProductToCurrentOrder()
    {
        if (s_currentOrder is null)
        {
            Console.WriteLine("Create an order first.");
            return;
        }

        var productId = ReadInt("Product id to add: ");
        var amount = ReadInt("Amount to add (negative to remove): ");

        try
        {
            var sales = s_bl.Order.AddProductToOrder(s_currentOrder, productId, amount);
            Console.WriteLine("Order updated.");
            Console.WriteLine("Applied sales:");
            foreach (var sale in sales)
                Console.WriteLine(sale.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to update order: {ex.Message}");
        }
    }

    private static void ShowCurrentOrder()
    {
        if (s_currentOrder is null)
        {
            Console.WriteLine("No current order.");
            return;
        }

        Console.WriteLine(s_currentOrder.ToString());
        foreach (var item in s_currentOrder.Items)
            Console.WriteLine(item.ToString());
    }

    private static void FinalizeOrder()
    {
        if (s_currentOrder is null)
        {
            Console.WriteLine("No current order.");
            return;
        }

        try
        {
            s_bl.Order.DoOrder(s_currentOrder);
            Console.WriteLine("Order finalized and stock updated.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to finalize order: {ex.Message}");
        }
    }

    private static int ReadInt(string prompt)
    {
        Console.Write(prompt);
        var value = Console.ReadLine();
        return int.TryParse(value, out var result) ? result : 0;
    }

    private static double ReadDouble(string prompt)
    {
        Console.Write(prompt);
        var value = Console.ReadLine();
        return double.TryParse(value, out var result) ? result : 0d;
    }

    private static string ReadString(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? string.Empty;
    }

    private static BO.Category ReadCategory(string prompt)
    {
        Console.Write(prompt);
        var value = Console.ReadLine()?.Trim();
        return Enum.TryParse<BO.Category>(value, true, out var category)
            ? category
                : BO.Category.LIGHTING;
    }

    private static bool ReadBool(string prompt)
    {
        Console.Write(prompt);
        var value = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(value)) return false;
        value = value.Trim().ToLower();
        return value == "y" || value == "yes" || value == "true";
    }

    private static DateTime ReadDate(string prompt)
    {
        Console.Write(prompt);
        var value = Console.ReadLine();
        return DateTime.TryParse(value, out var result) ? result : DateTime.Now;
    }

    private static void TestLogin()
    {
        Console.Write("Email or name to test (default 'Cashier'): ");
        var user = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(user)) user = "Cashier";
        Console.Write("Password (default 'cashier'): ");
        var pwd = Console.ReadLine() ?? string.Empty;

        try
        {
            var session = s_bl.Login(user, pwd);
            Console.WriteLine($"Login success: Id={session.Id}, Name={session.Name}, IsAdmin={session.IsAdmin}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login failed: {ex.Message}");
        }
    }
}
