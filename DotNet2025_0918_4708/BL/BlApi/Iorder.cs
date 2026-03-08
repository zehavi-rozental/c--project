using BO;

namespace BlApi;

public interface IOrder
{
    IEnumerable<SaleInProduct> SaleInProducts(Order order, int IdProduct, int ammountFororder);
    void CalcTotalPriceForProduct(ProductInOrder product);
    void CalcTotalPrice(Order order);
    void DoOrder(Order order);

    void SearchSaleForProduct(ProductInOrder product,bool IsPreferredCustomer);


}