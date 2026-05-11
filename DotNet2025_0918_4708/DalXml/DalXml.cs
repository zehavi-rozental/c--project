using DalApi;

namespace Dal;

internal sealed class DalXml : IDal
{
    public ICustomer Customer { get; } = new CustomerImplementation();
    public IProduct Product { get; } = new ProductImplementation();
    public ISale Sale { get; } = new SaleImplementation();

    private DalXml() { }

    private static readonly DalXml instance = new DalXml();

    public static IDal Instance { get; } = instance;
}