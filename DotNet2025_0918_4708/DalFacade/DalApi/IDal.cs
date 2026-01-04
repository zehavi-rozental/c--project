namespace DalApi;
public interface IDal
{
    ISale Sale { get; }
    ICustomer Customer { get; }
    IProduct Product { get; }
}
