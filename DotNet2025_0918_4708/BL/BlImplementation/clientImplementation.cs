using BlApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL.BlImplementation;

internal class clientImplementation : IClient
{
    private DalApi.IDal _dal = DalApi.Factory.Get;

    public int Create(BO.Client item)
    {
        try
        {
            return _dal.Customer.Create(BO.Tools.ToDo(item));
        }
        catch (Dal.IdAlreadyExistsException ex)
        {
            throw new BO.BLIdAlreadyExistsException("Client already exists.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BLException("Failed to create client.", ex);
        }
    }

    public BO.Client? Read(int id)
    {
        try
        {
            var customer = _dal.Customer.Read(id);
            return customer is null ? null : BO.Tools.ToBo(customer);
        }
        catch (Dal.IdNotFoundException ex)
        {
            throw new BO.BLIdNotFoundException($"Client {id} was not found.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BLException("Failed to read client.", ex);
        }
    }

    public IEnumerable<BO.Client> ReadAll()
    {
        try
        {
            return _dal.Customer.ReadAll().Select(c => BO.Tools.ToBo(c));
        }
        catch (Exception ex)
        {
            throw new BO.BLException("Failed to read clients.", ex);
        }
    }

    public void Update(BO.Client item)
    {
        try
        {
            _dal.Customer.Update(BO.Tools.ToDo(item));
        }
        catch (Dal.IdNotFoundException ex)
        {
            throw new BO.BLIdNotFoundException($"Client {item.Id} was not found.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BLException("Failed to update client.", ex);
        }
    }

    public void Delete(int id)
    {
        try
        {
            _dal.Customer.Delete(id);
        }
        catch (Dal.IdNotFoundException ex)
        {
            throw new BO.BLIdNotFoundException($"Client {id} was not found.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BLException("Failed to delete client.", ex);
        }
    }

    public bool IsExist(int id)
    {
        try
        {
            return _dal.Customer.Read(id) != null;
        }
        catch (Dal.IdNotFoundException)
        {
            return false;
        }
        catch (Exception ex)
        {
            throw new BO.BLException("Failed to verify client existence.", ex);
        }
    }
}
