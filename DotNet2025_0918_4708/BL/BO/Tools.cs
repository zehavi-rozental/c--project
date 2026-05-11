using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BO;

internal static class Tools
{
    public static string ToStringProperty<T>(this T obj)
    {
        if (obj == null)
            return string.Empty;

        var type = obj.GetType();
        var sb = new StringBuilder();
        sb.AppendLine(type.Name + ":");

        foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var value = prop.GetValue(obj, null);
            sb.Append(prop.Name + ": ");

            if (value is null)
            {
                sb.AppendLine("null");
                continue;
            }

            if (value is string)
            {
                sb.AppendLine(value.ToString());
                continue;
            }

            if (value is IEnumerable collection)
            {
                var items = collection.Cast<object>().Select(item => item?.ToString() ?? "null");
                sb.AppendLine("[" + string.Join(", ", items) + "]");
                continue;
            }

            sb.AppendLine(value.ToString());
        }

        return sb.ToString();
    }

    public static Client ToBo(this global::DO.Customer source)
        => new Client
        {
            Id = source.Id,
            Name = source.Name ?? string.Empty,
            Email = source.Email ?? string.Empty,
            Password = source.Password ?? string.Empty,
            Address = source.Address ?? string.Empty,
            PhoneNumber = source.PhoneNumber ?? string.Empty,
            IsAdmin = source.IsAdmin
        };

    public static global::DO.Customer ToDo(this Client source)
        => new global::DO.Customer(source.Id, source.Name, source.Email, source.Password, source.Address, source.PhoneNumber, source.IsAdmin);

    public static Product ToBo(this global::DO.Product source)
        => new Product
        {
            Id = source.Id,
            ProductName = source.ProductName,
            Category = (Category)source.Category,
            Price = source.Price,
            Ammount = source.Ammount
        };

    public static global::DO.Product ToDo(this Product source)
        => new global::DO.Product(source.Id, source.ProductName, (global::DO.Category)source.Category, source.Price, source.Ammount);

    public static Sale ToBo(this global::DO.Sale source)
        => new Sale
        {
            Id = source.Id,
            ProductId = source.ProductId,
            AmmontRequird = source.AmmontRequird,
            TotalPrice = source.TotalPrice,
            IsClubMembers = source.IsClubMembers,
            StartSale = source.StartSale,
            EndSale = source.EndSale
        };

    public static global::DO.Sale ToDo(this Sale source)
        => new global::DO.Sale(source.Id, source.ProductId, source.AmmontRequird, source.TotalPrice, source.IsClubMembers, source.StartSale, source.EndSale);

    public static SaleInProduct ToSaleInProduct(this global::DO.Sale sale)
        => new SaleInProduct
        {
            Id = sale.Id,
            AmountRequired = sale.AmmontRequird,
            SalePrice = sale.TotalPrice,
            IsForEveryone = sale.IsClubMembers
        };
}
    
  

