using System.Collections.Generic;
using DO;

namespace Dal;

internal static class DataSource
{
    internal static List<Product?> Products = new List<Product?>();
    internal static List<Sale?> Sales = new List<Sale?>();
    internal static List<Customer?> Customers = new List<Customer?>();

    internal static class config
    {
        internal const int initialId = 1000;
        private static int statiValue = initialId;
        public static int StaticValue { get { return statiValue++; } }
    }
}