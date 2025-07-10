using Nest;

namespace AuctionService.Repos;

public class ElasticGenericRepo<T> : IElasticGenericRepo<T> where T : class
{
    private readonly IElasticClient client;

    public ElasticGenericRepo(IElasticClient client)
    {
        this.client = client;
    }

    public async Task<bool> Delete(string id) => (await client.DeleteAsync<T>(id)).IsValid;

    public async Task<T> Get(string id) => (await client.GetAsync<T>(id)).Source;

    public async Task<IEnumerable<T>> GetAll() => (await client.SearchAsync<T>(s => s
        .MatchAll()
        .Size(10000) 
    )).Documents;

    public async Task<IEnumerable<string>> Index(IEnumerable<T> documents)
    {
        var indexName = typeof(T).Name.ToLower();
        var indexResponse = await client.Indices.ExistsAsync(indexName);

        if (!indexResponse.Exists)
            await client.Indices.CreateAsync(indexName, i => i.Map<T>(x => x.AutoMap()));

        var response = await client.IndexManyAsync(documents);
        return response.Items.Select(x => x.Id);
    }

    public async Task<bool> Update(T document, string id)
    {
        var response = await client.UpdateAsync<T>(id, u => u.Doc(document));
        return response.IsValid;
    }
}