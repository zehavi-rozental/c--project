using System.Collections.Generic;
using DalApi;
using Tools;
using DO;

namespace Dal;

internal class ProductImplementation : IProduct
{
    public int Create(Product product)
    {
        LogManager.Log("DalList", "ProductImplementation.Create", $"Attempting to create product with ID {product.Id}");
        if (DataSource.Products.Any(p => p?.Id == product.Id))
            throw new IdAlreadyExistsException("The ID " + product.Id + " already exists.");

        int newId = DataSource.config.StaticValue;
        Product newProduct = product with { Id = newId };

        DataSource.Products.Add(newProduct);
       LogManager.Log("DalList", "ProductImplementation.Create", $"Created product with ID {newId}");
        return newId;
    }

    public Product? Read(int id)
    {
        LogManager.Log("DalList", "ProductImplementation.Read", $"Attempting to read product with ID {id}");
        foreach (var product in DataSource.Products)
        {
            if (product?.Id == id)
                LogManager.Log("DalList", "ProductImplementation.Read", $"Read product with ID {id}");
                return product;
        }
        throw new IdNotFoundException();
    }

    public List<Product> ReadAll()
    {
        LogManager.Log("DalList", "ProductImplementation.ReadAll", "Attempting to read all products");
        return new List<Product>(DataSource.Products.Where(p => p != null));
    }

    public void Update(Product product)
    {
        LogManager.Log("DalList", "ProductImplementation.Update", $"Attempting to update product with ID {product.Id}");
        for (int i = 0; i < DataSource.Products.Count; i++)
        {
            if (DataSource.Products[i]?.Id == product.Id)
            {
                DataSource.Products[i] = product;
               LogManager.Log("DalList", "ProductImplementation.Update", $"Updated product with ID {product.Id}");
                return;
            }
        }
        throw new IdNotFoundException();
    }

    public void Delete(int id)
    {
        LogManager.Log("DalList", "ProductImplementation.Delete", $"Attempting to delete product with ID {id}");
        var product = DataSource.Products.FirstOrDefault(p => p?.Id == id);
        if (product == null)
            throw new IdNotFoundException();

        DataSource.Products.Remove(product);
        LogManager.Log("DalList", "ProductImplementation.Delete", $"Deleted product with ID {id}");
    }
}
