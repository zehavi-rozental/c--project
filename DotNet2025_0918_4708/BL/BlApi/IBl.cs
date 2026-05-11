namespace BlApi;

public interface IBl
{
    IClient Client { get; }
    IProduct Product { get; }
    ISale Sale { get; }
    IOrder Order { get; }
    BO.UserSession Login(string email, string password);
}
