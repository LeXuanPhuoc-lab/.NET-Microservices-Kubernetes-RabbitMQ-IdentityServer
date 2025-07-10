namespace AuctionService.Repos;
public interface IElasticGenericRepo<T>
{
    Task<IEnumerable<string>> Index(IEnumerable<T> documents);
    Task<T> Get(string id);
    Task<IEnumerable<T>> GetAll();  
    Task<bool> Update(T document, string id);
    Task<bool> Delete(string id);
}
