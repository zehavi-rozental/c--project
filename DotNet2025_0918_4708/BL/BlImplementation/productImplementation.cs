using BlApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL.BlImplementation;

internal class productImplementation : IProduct
{
    private DalApi.IDal _dal = DalApi.Factory.Get;

    public int Create(BO.Product item)
    {
        try
        {
            return _dal.Product.Create(BO.Tools.ToDo(item));
        }
        catch (Dal.IdAlreadyExistsException ex)
        {
            throw new BO.BLIdAlreadyExistsException("Product already exists.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BLException("Failed to create product.", ex);
        }
    }

    public BO.Product? Read(int id)
    {
        try
        {
            var product = _dal.Product.Read(id);
            return product is null ? null : BO.Tools.ToBo(product);
        }
        catch (Dal.IdNotFoundException ex)
        {
            throw new BO.BLIdNotFoundException($"Product {id} was not found.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BLException("Failed to read product.", ex);
        }
    }

    public IEnumerable<BO.Product> ReadAll()
    {
        try
        {
            return _dal.Product.ReadAll().Select(p => BO.Tools.ToBo(p));
        }
        catch (Exception ex)
        {
            throw new BO.BLException("Failed to read products.", ex);
        }
    }

    public void Update(BO.Product item)
    {
        try
        {
            _dal.Product.Update(BO.Tools.ToDo(item));
        }
        catch (Dal.IdNotFoundException ex)
        {
            throw new BO.BLIdNotFoundException($"Product {item.Id} was not found.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BLException("Failed to update product.", ex);
        }
    }

    public void Delete(int id)
    {
        try
        {
            _dal.Product.Delete(id);
        }
        catch (Dal.IdNotFoundException ex)
        {
            throw new BO.BLIdNotFoundException($"Product {id} was not found.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BLException("Failed to delete product.", ex);
        }
    }

    public void GetValidSales(BO.ProductInOrder productInOrder, bool isPreferredCustomer)
    {
        try
        {
            productInOrder.Sales = GetSalesForProduct(productInOrder, isPreferredCustomer).ToList();
        }
        catch (Exception ex)
        {
            throw new BO.BLException("Failed to get valid sales for product.", ex);
        }
    }

    private IEnumerable<BO.SaleInProduct> GetSalesForProduct(BO.ProductInOrder productInOrder, bool isPreferredCustomer)
    {
        var now = DateTime.Now;
        var validSales = from sale in _dal.Sale.ReadAll()
                        where sale.ProductId == productInOrder.ProductId
                           && sale.StartSale <= now
                           && sale.EndSale >= now
                           && productInOrder.Amount >= sale.AmmontRequird
                           && (isPreferredCustomer || sale.IsClubMembers == false)
                        orderby (double)sale.TotalPrice / sale.AmmontRequird
                        select BO.Tools.ToSaleInProduct(sale);

        return validSales;
    }
}
