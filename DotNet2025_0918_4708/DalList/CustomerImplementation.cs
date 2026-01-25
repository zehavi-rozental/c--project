using System.Linq;
using DalApi;
using DO;
using static Dal.DataSource;

namespace Dal;

internal class CustomerImplementation : ICustomer
{
    public int Create(Customer customer)
    {
        var q = from c in Customers
                where c?.Id == customer.Id
                select c;
        Customer cust = q.FirstOrDefault()!;
        if (cust != null)
        {
            throw new IdAlreadyExistsException("The ID " + customer.Id + " already exists.");
        }
        customer = customer with { Id = config.NextCustomerId };
        Customers.Add(customer);
        return customer.Id;
    }

    public Customer? Read(int id)
    {
        var r = from c in Customers
                where c?.Id == id
                select c;
        Customer customer = r.FirstOrDefault()!;
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
        Delete(customer.Id);
        Create(customer);
    }

    public void Delete(int id)
    {
        var d = from c in Customers
                where c?.Id == id
                select c;
        Customer customer = d.FirstOrDefault()!;
        if (d == null)
            throw new IdNotFoundException();
        Customers.Remove(customer);
    }
}