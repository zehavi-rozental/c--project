using DalApi;
using DO;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using Tools;

namespace Dal;

internal class CustomerImplementation : ICustomer
{
    // NOTE: Breaking change in DO.Customer schema. If you have existing serialized files
    // (xml/customers.xml) produced by the previous schema, please delete them from the
    // `bin` output folders (for example: bin\Debug\net8.0\xml\customers.xml) to avoid
    // XmlSerializer deserialization errors after this change.
    private static string ResolveFilePath()
    {
        // Try to locate a repository/solution root (contains a .sln) and prefer its xml folder.
        string? solutionRoot = null;
        var dirInfo = new DirectoryInfo(AppContext.BaseDirectory);
        while (dirInfo != null)
        {
            if (dirInfo.GetFiles("*.sln").Any())
            {
                solutionRoot = dirInfo.FullName;
                break;
            }
            dirInfo = dirInfo.Parent;
        }

        // Collect likely locations for the xml file; prefer solution-level xml when available.
        var candidates = new List<string>();
        if (!string.IsNullOrEmpty(solutionRoot))
            candidates.Add(Path.Combine(solutionRoot, "xml", "customers.xml"));

        candidates.Add(Path.Combine(AppContext.BaseDirectory, "xml", "customers.xml"));
        candidates.Add(Path.Combine(Directory.GetCurrentDirectory(), "xml", "customers.xml"));

        // Walk up parent folders to try to find other solution-relative xml folders
        var directoryInfo = new DirectoryInfo(AppContext.BaseDirectory);
        while (directoryInfo != null)
        {
            var candidate = Path.Combine(directoryInfo.FullName, "xml", "customers.xml");
            if (!candidates.Contains(candidate)) candidates.Add(candidate);
            directoryInfo = directoryInfo.Parent;
        }

        // Return the first existing candidate (priority order matters)
        var existing = candidates.Where(p => File.Exists(p)).Distinct().ToList();
        if (existing.Any()) return existing.First();

        // Default to AppContext.BaseDirectory/xml/customers.xml (will be created when saving)
        return Path.Combine(AppContext.BaseDirectory, "xml", "customers.xml");
    }

    private static List<Customer> LoadCustomers()
    {
        var path = ResolveFilePath();
        try { LogManager.Log("DalXml", "LoadCustomers", $"Resolved customers.xml path: {path}"); } catch { }

        if (!File.Exists(path))
        {
            // Initialize default customers (Admin + Cashier) to avoid blank DB for UI apps
            var initial = new List<Customer>
            {
                new Customer(1, "Admin", "admin", "123", "Main St", "0501234567", true),
                new Customer(2, "Cashier", "cashier", "cashier", "Second St", "0507654321", false),
                new Customer(3, "Manager", "manager", "123", "Main St", "0501234567", true)
            };
            // Ensure directory exists
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            SaveCustomers(initial);
            try { LogManager.Log("DalXml", "LoadCustomers", $"Created initial customers file with {initial.Count} entries"); } catch { }
            return initial;
        }

        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Customer>), new XmlRootAttribute("ArrayOfCustomer"));
            var customers = (List<Customer>)serializer.Deserialize(fs);
            try
            {
                var summary = string.Join("; ", customers.Select(c => $"{c.Name} (pwdLen={ (c.Password?.Length ?? 0) })"));
                LogManager.Log("DalXml", "LoadCustomers", $"Loaded {customers.Count} customers: {summary}");
            }
            catch { }
            return customers;
        }
    }

    private static void SaveCustomers(List<Customer> customers)
    {
        var path = ResolveFilePath();
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Customer>), new XmlRootAttribute("ArrayOfCustomer"));
            serializer.Serialize(fs, customers);
        }
    }

    public int Create(Customer item)
    {
        List<Customer> customers = LoadCustomers();
        customers.Add(item);
        SaveCustomers(customers);
        return item.Id;
    }

    public Customer? Read(int id)
    {
        List<Customer> customers = LoadCustomers();
        return customers.FirstOrDefault(c => c.Id == id);
    }

    public void Update(Customer item)
    {
        List<Customer> customers = LoadCustomers();
        int index = customers.FindIndex(c => c.Id == item.Id);
        if (index == -1) throw new Exception("Customer not found");
        customers[index] = item;
        SaveCustomers(customers);
    }

    public void Delete(int id)
    {
        List<Customer> customers = LoadCustomers();
        customers.RemoveAll(c => c.Id == id);
        SaveCustomers(customers);
    }

    public List<Customer> ReadAll()
    {
        return LoadCustomers();
    }
}