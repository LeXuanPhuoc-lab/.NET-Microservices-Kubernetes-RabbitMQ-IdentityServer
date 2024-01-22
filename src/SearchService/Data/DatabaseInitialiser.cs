using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core.Servers;
using MongoDB.Entities;
using SearchService.Entities;
using SearchService.Services;

namespace SearchService.Data
{
    public static class DatabaseInitialiserExtension
    {
        public static async Task InitailiseDatabaseAsync(this WebApplication app)
        {
            // Create IScopeService for resolve all service require
            using(var scope = app.Services.CreateScope())
            {
                var initialiser = scope.ServiceProvider.GetRequiredService<DatabaseInitialiser>();
            
                // Seed Data 
                await initialiser.SeedAsync(app);
            }
        }
    }

    public interface IDatabaseInitialiser
    {
        Task SeedAsync(WebApplication app);
        Task TrySeedAsync(WebApplication app);
    }

    public class DatabaseInitialiser : IDatabaseInitialiser
    {
        public async Task SeedAsync(WebApplication app)
        {
            try
            {
                await TrySeedAsync(app);
            }catch
            {
                throw;
            }
        }

        public async Task TrySeedAsync(WebApplication app)
        {
            try
            {
                var count = await DB.CountAsync<Auction>();

                if(count > 0) 
                {
                    Console.WriteLine("Already seed data");
                    return;
                }

                Console.WriteLine("No data - will atempt to seed data");

                /** Get data in JSON file */ 
                // // Read all data from json 
                // var itemData = await File.ReadAllTextAsync("Data/auctions.json");

                // //Console.WriteLine(itemData);
                
                // // Create some options for the json serialization
                // var options = new JsonSerializerOptions(){PropertyNameCaseInsensitive=true};

                // // Deserialize data into list of item
                // var items = JsonSerializer.Deserialize<List<Auction>>(itemData, options);

                // // Save data
                // if(items is not null) await DB.SaveAsync(items);


                /** Get Data from AuctionService */
                // Create IScopeService to resolve 
                using(var scope = app.Services.CreateScope())
                {
                    var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();

                    var items = await httpClient.GetItemsForSearchAsync();

                    if(items != null)
                    {
                        Console.WriteLine(items.Count + " return from the auction service");
                        await DB.SaveAsync(items);
                    }
                }
            }catch
            {
                throw;
            }
        }
    }
}