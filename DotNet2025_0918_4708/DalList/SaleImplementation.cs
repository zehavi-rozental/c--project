using System.Collections.Generic;
using System.Linq;
using DalApi;
using DO;
using Tools;
using static Dal.DataSource;

namespace Dal;

internal class SaleImplementation : ISale
{
    public int Create(Sale sale)
    {
        LogManager.Log("DalList", "SaleImplementation.Create", $"Attempting to create sale with ID {sale.Id}");
        if (Sales.Any(s => s?.Id == sale.Id))
            throw new IdAlreadyExistsException("The ID " + sale.Id + " already exists.");

        sale = sale with { Id = sale.Id == 0 ? config.StaticValue : sale.Id };
        Sales.Add(sale);
        LogManager.Log("DalList", "SaleImplementation.Create", $"Created sale with ID {sale.Id}");
        return sale.Id;
    }

    public Sale? Read(int id)
    {
        LogManager.Log("DalList", "SaleImplementation.Read", $"Attempting to read sale with ID {id}");
        var sale = Sales.FirstOrDefault(s => s?.Id == id);
        if (sale == null)
            throw new IdNotFoundException();
      LogManager.Log("DalList", "SaleImplementation.Read", $"Read sale with ID {id}");
        return sale;
    }

    public List<Sale> ReadAll()
    {
        LogManager.Log("DalList", "SaleImplementation.ReadAll", "Attempting to read all sales");
        return Sales.Where(s => s != null).ToList()!;
    }

    public void Update(Sale sale)
    {
        LogManager.Log("DalList", "SaleImplementation.Update", $"Attempting to update sale with ID {sale.Id}");
        var index = Sales.FindIndex(s => s?.Id == sale.Id);
        if (index == -1)
            throw new IdNotFoundException();

        Sales[index] = sale;
        LogManager.Log("DalList", "SaleImplementation.Update", $"Updated sale with ID {sale.Id}");
    }

    public void Delete(int id)
    {
        LogManager.Log("DalList", "SaleImplementation.Delete", $"Attempting to delete sale with ID {id}");
        var sale = Sales.FirstOrDefault(s => s?.Id == id);
        if (sale == null)
            throw new IdNotFoundException();

        Sales.Remove(sale);
        LogManager.Log("DalList", "SaleImplementation.Delete", $"Deleted sale with ID {id}");
    }
}