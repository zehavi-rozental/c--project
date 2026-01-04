using System.Collections.Generic;
using DalApi;
using DO;

namespace Dal;

internal class ProductImplementation : IProduct
{
    public int Create(Product product)
    {
        if (DataSource.Products.Any(p => p?.Id == product.Id))
            throw new IdAlreadyExistsException("The ID " + product.Id + " already exists.");

        product.Id= DataSource.config.StaticValue;
        DataSource.Products.Add(product);
        return product.Id;
    }

    public Product? Read(int id)
    {
        foreach (var product in DataSource.Products)
        {
            if (product?.Id == id)
                return product;
        }
        throw new IdNotFoundException();
    }

    public List<Product> ReadAll()
    {
        return new List<Product>(DataSource.Products.Where(p => p != null));
    }

    public void Update(Product product)
    {
        for (int i = 0; i < DataSource.Products.Count; i++)
        {
            if (DataSource.Products[i]?.Id == product.Id)
            {
                DataSource.Products[i] = product;
                return;
            }
        }
        throw new IdNotFoundException();
    }

    public void Delete(int id)
    {
        var product = DataSource.Products.FirstOrDefault(p => p?.Id == id);
        if (product == null)
            throw new IdNotFoundException();

        DataSource.Products.Remove(product);
    }
}
