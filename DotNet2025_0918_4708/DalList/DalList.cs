using System;
using DalApi;

namespace Dal;

internal class DalList : IDal
{
    public ISale Sale => SaleImplementation();
    public IProduct Product => ProductImplementation();
    public ICustomer Customer => CustomerImplementation();

   
}