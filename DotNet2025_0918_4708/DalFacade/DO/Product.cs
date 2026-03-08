namespace DO;

public record Product(
    int Id, 
    string ProductName, 
    Category Category, 
    double Price, 
    int Ammount)
{

}