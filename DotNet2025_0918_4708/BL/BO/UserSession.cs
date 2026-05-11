namespace BO;

// אובייקט קטן שמחזיק את נתוני המשתמש המחובר כרגע
public class UserSession
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsAdmin { get; set; }
}