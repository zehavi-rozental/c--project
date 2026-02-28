using System.Collections.Generic;
using System.Linq;
using DalApi;
using DO;
using static Dal.DataSource;

namespace Dal;

internal class SaleImplementation : ISale
{
    public int Create(Sale sale)
    {
        if (Sales.Any(s => s?.Id == sale.Id))
            throw new IdAlreadyExistsException("The ID " + sale.Id + " already exists.");

        sale = sale with { Id = sale.Id == 0 ? config.StaticValue : sale.Id };
        Sales.Add(sale);
        return sale.Id;
    }

    public Sale? Read(int id)
    {
        var sale = Sales.FirstOrDefault(s => s?.Id == id);
        if (sale == null)
            throw new IdNotFoundException();
        return sale;
    }

    public List<Sale> ReadAll()
    {
        return Sales.Where(s => s != null).ToList()!;
    }

    public void Update(Sale sale)
    {
        var index = Sales.FindIndex(s => s?.Id == sale.Id);
        if (index == -1)
            throw new IdNotFoundException();

        Sales[index] = sale;
    }

    public void Delete(int id)
    {
        var sale = Sales.FirstOrDefault(s => s?.Id == id);
        if (sale == null)
            throw new IdNotFoundException();

        Sales.Remove(sale);
    }
}