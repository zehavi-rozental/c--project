using System.Linq;
using DalApi;
using DO;
using Tools;    
using static Dal.DataSource;

namespace Dal;

internal class CustomerImplementation : ICustomer
{
    public int Create(Customer customer)
    {
        LogManager.Log("DalList", "CustomerImplementation.Create", $"Attempting to create customer with ID {customer.Id}");
        if (Customers.Any(c => c?.Id == customer.Id))
            throw new IdAlreadyExistsException("The ID " + customer.Id + " already exists.");

        // שימוש ב-ID האוטומטי רק אם ה-ID שהתקבל הוא 0 (או ערך ברירת מחדל אחר)
        customer.Id = customer.Id == 0 ? config.StaticValue : customer.Id;
        Customers.Add(customer);
        LogManager.Log("DalList", "CustomerImplementation.Create", $"Created customer with ID {customer.Id}");
        return customer.Id;
    }

    public Customer? Read(int id)
    {
        LogManager.Log("DalList", "CustomerImplementation.Read", $"Attempting to read customer with ID {id}");
        var customer = Customers.FirstOrDefault(c => c?.Id == id);
        if (customer == null)
            throw new IdNotFoundException();
        LogManager.Log("DalList", "CustomerImplementation.Read", $"Read customer with ID {id}");
        return customer;
    }

    public List<Customer> ReadAll()
    {
        LogManager.Log("DalList", "CustomerImplementation.ReadAll", "Attempting to read all customers");
        return Customers.Where(c => c != null).ToList()!;
    }

    public void Update(Customer customer)
    {
        LogManager.Log("DalList", "CustomerImplementation.Update", $"Attempting to update customer with ID {customer.Id}");
        var index = Customers.FindIndex(c => c?.Id == customer.Id);
        if (index == -1)
            throw new IdNotFoundException();

        Customers[index] = customer;
        LogManager.Log("DalList", "CustomerImplementation.Update", $"Updated customer with ID {customer.Id}");
    }

    public void Delete(int id)
    {
            LogManager.Log("DalList", "CustomerImplementation.Delete", $"Attempting to delete customer with ID {id}");
        var customer = Customers.FirstOrDefault(c => c?.Id == id);
        if (customer == null)
            throw new IdNotFoundException();

        Customers.Remove(customer);
        LogManager.Log("DalList", "CustomerImplementation.Delete", $"Deleted customer with ID {id}");
    }
}