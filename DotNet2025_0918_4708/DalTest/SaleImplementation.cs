using System.Collections.Generic;
using DalApi;
using DO;

namespace Dal;

internal class Sale : ISale
{
    public int Id { get; private set; } 

    public int Create(Sale sale)
    {
        if (DataSource.Sales.Any(s => s?.Id == sale.Id))
            throw new IdAlreadyExistsException("The ID " + sale.Id + " already exists.");

        sale.Id = DataSource.config.StaticValue;
        DataSource.Sales.Add(sale);
        return sale.Id;
    }

    public Sale? Read(int id)
    {
        foreach (var sale in DataSource.Sales)
        {
            if (sale?.Id == id)
                return sale;
        }
        throw new IdNotFoundException();
    }

    public List<Sale> ReadAll()
    {
        return new List<Sale>(DataSource.Sales.Where(s => s != null));
    }

    public void Update(Sale sale)
    {
        for (int i = 0; i < DataSource.Sales.Count; i++)
        {
            if (DataSource.Sales[i]?.Id == sale.Id)
            {
                DataSource.Sales[i] = sale;
                return;
            }
        }
        throw new IdNotFoundException();
    }

    public void Delete(int id)
    {
        var sale = DataSource.Sales.FirstOrDefault(s => s?.Id == id);
        if (sale == null)
            throw new IdNotFoundException();

        DataSource.Sales.Remove(sale);
    }
}