using DalApi;

namespace Dal;

public class DalList : IDal
{
    public ISale Sale => new SaleImplementation();
    public IProduct Product => new ProductImplementation();
    public ICustomer Customer => new CustomerImplementation();
}