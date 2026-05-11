using System.IO;
using System.Xml.Linq;

namespace Dal;

internal static class Config
{
    private static readonly string ConfigFileName = "data-config";
    private static readonly string s_configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "xml", "data-config.xml");

    public static int CustomerNum
    {
        get
        {
            XElement root = XElement.Load(s_configFilePath);
            int num = (int)root.Element("CustomerNum");
            root.Element("CustomerNum").SetValue(num + 1);
            root.Save(s_configFilePath);
            return num;
        }
        private set { /* write to XML */ }
    }

    public static int ProductNum
    {
        get
        {
            XElement root = XElement.Load(s_configFilePath);
            int num = (int)root.Element("ProductNum");
            root.Element("ProductNum").SetValue(num + 1);
            root.Save(s_configFilePath);
            return num;
        }
        private set { /* write to XML */ }
    }

    public static int SaleNum
    {
        get
        {
            XElement root = XElement.Load(s_configFilePath);
            int num = (int)root.Element("SaleNum");
            root.Element("SaleNum").SetValue(num + 1);
            root.Save(s_configFilePath);
            return num;
        }
        private set { /* write to XML */ }
    }
}
