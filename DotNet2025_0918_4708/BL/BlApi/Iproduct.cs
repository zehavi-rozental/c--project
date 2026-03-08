namespace BlApi;

public interface IProduct
{
    int Create(BO.Product item);
    BO.Product? Read(int id);
    IEnumerable<BO.Product> ReadAll();
    void Update(BO.Product item);
    void Delete(int id);
    void GetValidSales(BO.ProductInOrder productInOrder, bool isPreferred);
}