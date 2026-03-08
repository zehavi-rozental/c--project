namespace BlApi;

public interface ISale
{
    int Create(BO.Sale item);
    BO.Sale? Read(int id);
    IEnumerable<BO.Sale> ReadAll();
    void Update(BO.Sale item);
    void Delete(int id);
}