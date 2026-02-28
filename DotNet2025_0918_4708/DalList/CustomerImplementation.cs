using System.Linq;
using DalApi;
using DO;
using static Dal.DataSource;

namespace Dal;

internal class CustomerImplementation : ICustomer
{
    public int Create(Customer customer)
    {
        if (Customers.Any(c => c?.Id == customer.Id))
            throw new IdAlreadyExistsException("The ID " + customer.Id + " already exists.");

        // שימוש ב-ID האוטומטי רק אם ה-ID שהתקבל הוא 0 (או ערך ברירת מחדל אחר)
        customer.Id = customer.Id == 0 ? config.StaticValue : customer.Id;
        Customers.Add(customer);
        return customer.Id;
    }

    public Customer? Read(int id)
    {
        var customer = Customers.FirstOrDefault(c => c?.Id == id);
        if (customer == null)
            throw new IdNotFoundException();
        return customer;
    }

    public List<Customer> ReadAll()
    {
        return Customers.Where(c => c != null).ToList()!;
    }

    public void Update(Customer customer)
    {
        var index = Customers.FindIndex(c => c?.Id == customer.Id);
        if (index == -1)
            throw new IdNotFoundException();

        Customers[index] = customer;
    }

    public void Delete(int id)
    {
        var customer = Customers.FirstOrDefault(c => c?.Id == id);
        if (customer == null)
            throw new IdNotFoundException();

        Customers.Remove(customer);
    }
}