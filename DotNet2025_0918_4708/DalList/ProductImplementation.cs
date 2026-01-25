using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using static Dal.DataSource;

namespace Dal;

internal class ProductImplementation : IProduct
{
    public int Create(Product product)
    {
        var q = from p in DataSource.Products
                where p?.Id == product.Id
                select p;
        Product prod = q.FirstOrDefault()!;
        if (prod != null)
            throw new IdAlreadyExistsException("The ID " + product.Id + " already exists.");
        product = product with { Id = config.NextProductId };
        DataSource.Products.Add(product);
        return product.Id;
    }

    public Product? Read(int id)
    {
        var r = from p in DataSource.Products
                where p?.Id == id
                select p;
        Product product = r.FirstOrDefault()!;
        if (product == null)
            throw new IdNotFoundException();
        return product;
    }

    public List<Product> ReadAll()
    {
        return DataSource.Products.Where(p => p != null).ToList()!;
    }

    public void Update(Product product)
    {
        Delete(product.Id);
        Create(product);
    }

    public void Delete(int id)
    {
        var d = from p in DataSource.Products
                where p?.Id == id
                select p;
        Product product = d.FirstOrDefault()!;
        if (product == null)
            throw new IdNotFoundException();
        DataSource.Products.Remove(product);
    }
}
