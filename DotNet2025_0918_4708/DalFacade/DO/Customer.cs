namespace DO;

public record Customer
(
    int Id,
    string? Name,
    string? Email,
    string? Password,
    string? Address,
    string? PhoneNumber,
    bool IsAdmin
)
{
    // חשוב ל-XML: קונסטרקטור ריק עם ערכי ברירת מחדל
    public Customer() : this(0, null, null, "1234", null, null, false) { }
}