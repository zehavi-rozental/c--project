using BO;

namespace BlApi;

public interface IOrder
{
    IEnumerable<SaleInProduct> AddProductToOrder(Order order, int productId, int ammountForOrder);
    void CalcTotalPriceForProduct(ProductInOrder product);
    void CalcTotalPrice(Order order);
    void DoOrder(Order order);
    void SearchSaleForProduct(ProductInOrder product, bool isPreferredCustomer);
}