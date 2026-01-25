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
        var q = from s in Sales
                where s?.Id == sale.Id
                select s;
        Sale sl = q.FirstOrDefault()!;
        if (sl != null)
            throw new IdAlreadyExistsException("The ID " + sale.Id + " already exists.");
        sale = sale with { Id = config.NextSaleId };
        Sales.Add(sale);
        return sale.Id;
    }

    public Sale? Read(int id)
    {
        var r = from s in Sales
                where s?.Id == id
                select s;
        Sale sale = r.FirstOrDefault()!;
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
        Delete(sale.Id);
        Create(sale);
    }

    public void Delete(int id)
    {
        var d = from s in Sales
                where s?.Id == id
                select s;
        Sale sale = d.FirstOrDefault()!;
        if (sale == null)
            throw new IdNotFoundException();
        Sales.Remove(sale);
    }
}