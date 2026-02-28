namespace DO;

public record Product(int Id, string ProductName, Category Category, double Price, int Ammount)
{
    public Product() : this(1, "G7", Category.CAMERAS, 3.4, 5)
    {

    }

}