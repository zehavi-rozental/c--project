namespace BlApi;

public interface IClient
{
    int Create(BO.Client item);     
    BO.Client? Read(int id);      
    IEnumerable<BO.Client> ReadAll(); 
    void Update(BO.Client item);  
    void Delete(int id);           
    bool IsExist(int id);          
}