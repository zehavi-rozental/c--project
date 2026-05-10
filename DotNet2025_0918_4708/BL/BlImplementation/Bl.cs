using BlApi;

namespace BL.BlImplementation;

internal class Bl : IBl
{
    public IClient Client => new clientImplementation();
    public IProduct Product => new productImplementation();
    public ISale Sale => new saleImplementation();
    public IOrder Order => new orderImplementation();
}
