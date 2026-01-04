using DalApi;
using DO;
using static Dal.DataSource;

namespace Dal;

internal class CustomerImplementation : ICustomer
{

    public int Create(Customer customer)
    {
        if (DataSource.Customers.Any(c => c?.Id == customer.Id))
            throw new IdAlreadyExistsException("The ID " + customer.Id + " already exists.");
        
        customer.Id = DataSource.config.StaticValue;
        DataSource.Customers.Add(customer);
        return customer.Id;
    }

    public Customer? Read(int id)
    {
        foreach (var c in DataSource.Customers)
        {
            if (c?.Id == id)
                return c;
        }
        throw new IdNotFoundException();
    }
    public List<Customer> ReadAll()
    {
        return new List<Customer>(DataSource.Customers);
    }

    public void Update(Customer customer)
    {
        for (int i = 0; i < DataSource.Customers.Count; i++)
        {
            if (customer.Id == DataSource.Customers[i]?.Id)
            {
                DataSource.Customers[i] = customer;
                return;
            }
        }
        throw new IdNotFoundException();
    }

    public void Delete(int id)
    {
        var customer = DataSource.Customers.FirstOrDefault(c => c?.Id == id);
        if (customer == null)
            throw new IdNotFoundException();

        DataSource.Customers.Remove(customer);
    }
}