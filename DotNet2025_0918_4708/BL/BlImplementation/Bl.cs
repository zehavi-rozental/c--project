using BlApi;
using DalApi;
using DO;
using System;
using System.Linq; // חשוב עבור FirstOrDefault
using Tools;

namespace BL.BlImplementation;

internal class Bl : IBl
{
    // גישה לשכבת הנתונים
    private readonly global::DalApi.IDal _dal = global::DalApi.Factory.Get;

    // מימושים של שאר הישויות
    public global::BlApi.IClient Client => new clientImplementation();
    public global::BlApi.IProduct Product => new productImplementation();
    public global::BlApi.ISale Sale => new saleImplementation();
    public global::BlApi.IOrder Order => new orderImplementation();

    // פונקציית ההתחברות
    public BO.UserSession Login(string email, string password)
    {
        // שליפת הנתונים מה-DAL — תמיכה גם בשם משתמש (Name) וגם ב־Email
        var emailTrim = email?.Trim() ?? string.Empty;
        var passwordTrim = password?.Trim() ?? string.Empty;

        try { LogManager.Log("BL", "Login", $"Attempt login '{emailTrim}' pwdLen={passwordTrim.Length}"); } catch { }

        Customer? customer = null;
        try
        {
            var all = _dal.Customer.ReadAll();
            foreach (var c in all)
            {
                bool emailMatch = !string.IsNullOrEmpty(c.Email) && string.Equals(c.Email.Trim(), emailTrim, StringComparison.OrdinalIgnoreCase);
                bool nameMatch = !string.IsNullOrEmpty(c.Name) && string.Equals(c.Name.Trim(), emailTrim, StringComparison.OrdinalIgnoreCase);
                bool pwdMatch = (c.Password?.Trim() ?? string.Empty) == passwordTrim;
                try { LogManager.Log("BL", "Login", $"Inspect Customer Id={c.Id} Name='{c.Name}' Email='{c.Email}' nameMatch={nameMatch} emailMatch={emailMatch} pwdMatch={pwdMatch}"); } catch { }
                if ((emailMatch || nameMatch) && pwdMatch)
                {
                    customer = c;
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            try { LogManager.Log("BL", "Login", "Error reading customers: " + ex.Message); } catch { }
            throw;
        }

        // בדיקה האם נמצא משתמש
        if (customer == null)
        {
            // אם BO.BlInvalidInputException גורם לשגיאה, החליפי ל-Exception רגיל
            throw new Exception("שם משתמש או סיסמה שגויים");
        }

        // החזרת הסשן למסך
        return new BO.UserSession
        {
            Id = customer.Id,
            Name = customer.Name ?? string.Empty,
            IsAdmin = customer.IsAdmin
        };
    }
}