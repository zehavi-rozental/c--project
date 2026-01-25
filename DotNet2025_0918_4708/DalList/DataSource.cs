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

        private static int _productId = initialId;
        private static int _customerId = initialId;
        private static int _saleId = initialId;

        public static int NextProductId => _productId++;
        public static int NextCustomerId => _customerId++;
        public static int NextSaleId => _saleId++;
    }
}