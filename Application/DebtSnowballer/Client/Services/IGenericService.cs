namespace DebtSnowballer.Client.Services;

public interface IGenericService<T>
{
    Task<List<T>> GetItems();
    Task<T> GetItem(int id);
    Task<T> AddItem(T item);
    Task<T> UpdateItem(T item);
    Task DeleteItem(int id);
}