using BlApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL.BlImplementation;

internal class orderImplementation : IOrder
{
    private DalApi.IDal _dal = DalApi.Factory.Get;

    public IEnumerable<BO.SaleInProduct> AddProductToOrder(BO.Order order, int productId, int ammountForOrder)
    {
        if (order is null)
            throw new BO.BLInvalidInputException("Order cannot be null.");

        if (ammountForOrder == 0)
            return Enumerable.Empty<BO.SaleInProduct>();

        try
        {
            var product = _dal.Product.Read(productId) ?? throw new BO.BLIdNotFoundException($"Product {productId} was not found.");
            var orderItem = order.Items.FirstOrDefault(item => item.ProductId == productId);

            if (orderItem is null)
            {
                if (ammountForOrder < 0)
                    throw new BO.BLInvalidInputException("Cannot reduce quantity for a missing product in the order.");

                if (product.Ammount < ammountForOrder)
                    throw new BO.BLInsufficientStockException("Not enough stock to add the requested quantity.");

                orderItem = new BO.ProductInOrder
                {
                    ProductId = product.Id,
                    ProductName = product.ProductName,
                    BasePrice = product.Price,
                    Amount = ammountForOrder
                };

                order.Items.Add(orderItem);
            }
            else
            {
                var targetAmount = orderItem.Amount + ammountForOrder;
                if (targetAmount < 0)
                    throw new BO.BLInvalidInputException("Order quantity cannot be negative.");

                if (targetAmount == 0)
                {
                    order.Items.Remove(orderItem);
                    CalcTotalPrice(order);
                    return Enumerable.Empty<BO.SaleInProduct>();
                }

                if (product.Ammount < targetAmount)
                    throw new BO.BLInsufficientStockException("Not enough stock to adjust the requested quantity.");

                orderItem.Amount = targetAmount;
            }

            SearchSaleForProduct(orderItem, order.IsPreferredCustomer);
            CalcTotalPriceForProduct(orderItem);
            CalcTotalPrice(order);
            return orderItem.Sales.ToList();
        }
        catch (Dal.IdNotFoundException ex)
        {
            throw new BO.BLIdNotFoundException($"Product {productId} was not found.", ex);
        }
        catch (BO.BLException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BO.BLException("Failed to add product to order.", ex);
        }
    }

    public void CalcTotalPriceForProduct(BO.ProductInOrder product)
{
    if (product is null)
        throw new BO.BLInvalidInputException("Product in order cannot be null.");

    int n = product.Amount;
    var sales = product.Sales;

    var dp = new double[n + 1];
    var choice = new BO.SaleInProduct?[n + 1];

    for (int i = 1; i <= n; i++)
    {
        dp[i] = dp[i - 1] + product.BasePrice;
        choice[i] = null;

        foreach (var sale in sales)
        {
            int qty = sale.AmountRequired;
            if (i >= qty && dp[i - qty] + sale.SalePrice < dp[i])
            {
                dp[i] = dp[i - qty] + sale.SalePrice;
                choice[i] = sale;
            }
        }
    }

    var usedSales = new List<BO.SaleInProduct>();
    int remaining = n;
    while (remaining > 0)
    {
        var selected = choice[remaining];
        if (selected is not null)
        {
            if (!usedSales.Contains(selected))
                usedSales.Add(selected);
            remaining -= selected.AmountRequired;
        }
        else
        {
            remaining--;
        }
    }

    product.Sales = usedSales;
    product.FinalPrice = dp[n];
}
    public void CalcTotalPrice(BO.Order order)
    {
        if (order is null)
            throw new BO.BLInvalidInputException("Order cannot be null.");

        order.TotalPrice = order.Items.Sum(item => item.FinalPrice);
    }

    public void DoOrder(BO.Order order)
    {
        if (order is null)
            throw new BO.BLInvalidInputException("Order cannot be null.");

        try
        {
            foreach (var item in order.Items)
            {
                if (item.Amount <= 0)
                    continue;

                var product = _dal.Product.Read(item.ProductId) ?? throw new BO.BLIdNotFoundException($"Product {item.ProductId} was not found.");
                if (product.Ammount < item.Amount)
                    throw new BO.BLInsufficientStockException($"Not enough stock for product {item.ProductId}.");

                _dal.Product.Update(new global::DO.Product(product.Id, product.ProductName, product.Category, product.Price, product.Ammount - item.Amount));
            }

            order.OrderDate = DateTime.Now;
        }
        catch (Dal.IdNotFoundException ex)
        {
            throw new BO.BLIdNotFoundException(ex.Message, ex);
        }
        catch (BO.BLException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BO.BLException("Failed to complete the order.", ex);
        }
    }

    public void SearchSaleForProduct(BO.ProductInOrder product, bool isPreferredCustomer)
    {
        if (product is null)
            throw new BO.BLInvalidInputException("Product in order cannot be null.");

        product.Sales = GetSalesForProduct(product, isPreferredCustomer).ToList();
    }

    private IEnumerable<BO.SaleInProduct> GetSalesForProduct(BO.ProductInOrder product, bool isPreferredCustomer)
    {
        var now = DateTime.Now;
        var validSales = from sale in _dal.Sale.ReadAll()
                        where sale.ProductId == product.ProductId
                           && sale.StartSale <= now
                           && sale.EndSale >= now
                           && product.Amount >= sale.AmmontRequird
                           && (isPreferredCustomer || sale.IsClubMembers == false)
                        orderby (double)sale.TotalPrice / sale.AmmontRequird
                        select BO.Tools.ToSaleInProduct(sale);

        return validSales;
    }
}
