using BlApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL.BlImplementation;

internal class saleImplementation : ISale
{
    private DalApi.IDal _dal = DalApi.Factory.Get;

    public int Create(BO.Sale item)
    {
        try
        {
            return _dal.Sale.Create(BO.Tools.ToDo(item));
        }
        catch (Dal.IdAlreadyExistsException ex)
        {
            throw new BO.BLIdAlreadyExistsException("Sale already exists.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BLException("Failed to create sale.", ex);
        }
    }

    public BO.Sale? Read(int id)
    {
        try
        {
            var sale = _dal.Sale.Read(id);
            return sale is null ? null : BO.Tools.ToBo(sale);
        }
        catch (Dal.IdNotFoundException ex)
        {
            throw new BO.BLIdNotFoundException($"Sale {id} was not found.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BLException("Failed to read sale.", ex);
        }
    }

    public IEnumerable<BO.Sale> ReadAll()
    {
        try
        {
            return _dal.Sale.ReadAll().Select(s => BO.Tools.ToBo(s));
        }
        catch (Exception ex)
        {
            throw new BO.BLException("Failed to read sales.", ex);
        }
    }

    public void Update(BO.Sale item)
    {
        try
        {
            _dal.Sale.Update(BO.Tools.ToDo(item));
        }
        catch (Dal.IdNotFoundException ex)
        {
            throw new BO.BLIdNotFoundException($"Sale {item.Id} was not found.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BLException("Failed to update sale.", ex);
        }
    }

    public void Delete(int id)
    {
        try
        {
            _dal.Sale.Delete(id);
        }
        catch (Dal.IdNotFoundException ex)
        {
            throw new BO.BLIdNotFoundException($"Sale {id} was not found.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BLException("Failed to delete sale.", ex);
        }
    }
}
