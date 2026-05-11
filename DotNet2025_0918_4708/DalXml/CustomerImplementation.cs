using DalApi;
using DO;
using System.Xml.Serialization;
using System.IO;

namespace Dal;

internal class CustomerImplementation : ICustomer
{
    private static readonly string s_filePath = Path.Combine(Directory.GetCurrentDirectory(), "xml", "customers.xml");

    private static List<Customer> LoadCustomers()
    {
        if (!File.Exists(s_filePath))
        {
            return new List<Customer>();
        }
        using (FileStream fs = new FileStream(s_filePath, FileMode.Open))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Customer>), new XmlRootAttribute("ArrayOfCustomer"));
            return (List<Customer>)serializer.Deserialize(fs);
        }
    }

    private static void SaveCustomers(List<Customer> customers)
    {
        using (FileStream fs = new FileStream(s_filePath, FileMode.Create))
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