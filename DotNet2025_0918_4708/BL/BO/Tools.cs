using BlApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BL.BO;

internal static class Tools
{
    public static string ToStringProperty<T>(this T obj)
    {
        string str = $"\n{obj.GetType().Name}:";
        foreach (PropertyInfo prop in obj.GetType().GetProperties())
        {
            var value = prop.GetValue(obj, null);
            if (value is IEnumerable collection && value is not string)
                str += $"\n{prop.Name}: " + string.Join(", ", collection.Cast<object>());
            else
                str += $"\n{prop.Name}: {value}";
        }
        return str;
    }
    public static TTarget CopyPropertiesTo<TTarget>(this object source) where TTarget : new()
    {
        // יוצרים אובייקט חדש מהסוג שאליו רוצים להמיר (למשל BO.Product)
        TTarget target = new TTarget();

        // עוברים על כל התכונות של אובייקט המקור (ה-DO)
        foreach (var sourceProp in source.GetType().GetProperties())
        {
            // מחפשים באובייקט היעד (ה-BO) תכונה עם אותו שם בדיוק
            var targetProp = typeof(TTarget).GetProperty(sourceProp.Name);

            // אם מצאנו תכונה כזו והיא ניתנת לכתיבה
            if (targetProp != null && targetProp.CanWrite)
            {
                // מעתיקים את הערך מהמקור ליעד
                targetProp.SetValue(target, sourceProp.GetValue(source));
            }
        }
        return target;
    }
}
    
  

