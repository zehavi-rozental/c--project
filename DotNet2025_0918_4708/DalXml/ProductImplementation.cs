using DalApi;
using DO;
using System.Xml.Serialization;
using System.IO;

namespace Dal;

internal class ProductImplementation : IProduct
{
    private static readonly string s_filePath = Path.Combine(Directory.GetCurrentDirectory(), "xml", "products.xml");

    private static List<Product> LoadProducts()
    {
        if (!File.Exists(s_filePath))
        {
            return new List<Product>();
        }
        using (FileStream fs = new FileStream(s_filePath, FileMode.Open))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Product>), new XmlRootAttribute("ArrayOfProduct"));
            return (List<Product>)serializer.Deserialize(fs);
        }
    }

    private static void SaveProducts(List<Product> products)
    {
        using (FileStream fs = new FileStream(s_filePath, FileMode.Create))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Product>), new XmlRootAttribute("ArrayOfProduct"));
            serializer.Serialize(fs, products);
        }
    }

    public int Create(Product item)
    {
        List<Product> products = LoadProducts();
        products.Add(item);
        SaveProducts(products);
        return item.Id;
    }

    public Product? Read(int id)
    {
        List<Product> products = LoadProducts();
        return products.FirstOrDefault(p => p.Id == id);
    }

    public void Update(Product item)
    {
        List<Product> products = LoadProducts();
        int index = products.FindIndex(p => p.Id == item.Id);
        if (index == -1) throw new Exception("Product not found");
        products[index] = item;
        SaveProducts(products);
    }

    public void Delete(int id)
    {
        List<Product> products = LoadProducts();
        products.RemoveAll(p => p.Id == id);
        SaveProducts(products);
    }

    public List<Product> ReadAll()
    {
        return LoadProducts();
    }
}