using DalApi;
using DO;
using System.Xml.Linq;
using System.IO;

namespace Dal;

internal class SaleImplementation : ISale
{
    private static readonly string s_filePath = Path.Combine(Directory.GetCurrentDirectory(), "xml", "sales.xml");

    private static XElement LoadSales()
    {
        return XElement.Load(s_filePath);
    }

    private static void SaveSales(XElement root)
    {
        root.Save(s_filePath);
    }

    private static Sale ElementToSale(XElement e)
    {
        return new Sale(
            (int)e.Element("Id"),
            (int)e.Element("ProductId"),
            (int)e.Element("AmmontRequird"),
            (double)e.Element("TotalPrice"),
            (bool)e.Element("IsClubMembers"),
            DateTime.Parse((string)e.Element("StartSale")),
            DateTime.Parse((string)e.Element("EndSale"))
        );
    }

    private static XElement SaleToElement(Sale s)
    {
        return new XElement("Sale",
            new XElement("Id", s.Id),
            new XElement("ProductId", s.ProductId),
            new XElement("AmmontRequird", s.AmmontRequird),
            new XElement("TotalPrice", s.TotalPrice),
            new XElement("IsClubMembers", s.IsClubMembers),
            new XElement("StartSale", s.StartSale.ToString("o")),
            new XElement("EndSale", s.EndSale.ToString("o"))
        );
    }

    public int Create(Sale item)
{
    XElement root = LoadSales();
    bool exists = root.Elements("Sale").Any(e => (int)e.Element("Id") == item.Id);
    if (exists)
        throw new Dal.IdAlreadyExistsException($"Sale with Id {item.Id} already exists.");
    root.Add(SaleToElement(item));
    SaveSales(root);
    return item.Id;
}

    public Sale? Read(int id)
    {
        XElement root = LoadSales();
        XElement? e = root.Elements("Sale").FirstOrDefault(e => (int)e.Element("Id") == id);
        return e == null ? null : ElementToSale(e);
    }

    public void Update(Sale item)
    {
        XElement root = LoadSales();
        XElement? e = root.Elements("Sale").FirstOrDefault(e => (int)e.Element("Id") == item.Id);
        if (e == null) throw new Exception("Sale not found");
        e.ReplaceWith(SaleToElement(item));
        SaveSales(root);
    }

    public void Delete(int id)
    {
        XElement root = LoadSales();
        XElement? e = root.Elements("Sale").FirstOrDefault(e => (int)e.Element("Id") == id);
        if (e != null) e.Remove();
        SaveSales(root);
    }

    public List<Sale> ReadAll()
    {
        XElement root = LoadSales();
        return root.Elements("Sale").Select(ElementToSale).ToList();
    }
}