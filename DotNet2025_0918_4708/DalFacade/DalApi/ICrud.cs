using DO;
namespace DalApi;
public interface ICrud<T>
{
    int Create(T item); //Creates new entity object in DAL
    T? Read(int id); //Reads entity object by its ID 
    List<T> ReadAll(); //Reads all entity objects
    void Update(T item); //Updates entity object
    void Delete(int id); //Deletes entity object by its ID
}