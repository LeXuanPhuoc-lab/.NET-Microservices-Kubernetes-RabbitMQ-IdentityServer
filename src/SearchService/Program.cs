using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Data;
using SearchService.Entities;
using SearchService.Extensions;
using SearchService.Services;
using SearchService.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add HttpClient, HttpClient Polly to handler error or connection refused 
builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(HttpClientExtension.GetPolicy());

// Config Appsettings
builder.Services.Configure<AppSettings>(
  builder.Configuration.GetSection("AppSettings")
);

// Add MongoDB
DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(
    builder.Configuration.GetConnectionString("MongoDB")))
  .Wait();

// Create an index for item 
DB.Index<Auction>() // Define the key first and then create()
  // Define search properties as key 
  .Key(x => x.Item.Make, KeyType.Text)
  .Key(x => x.Item.Model, KeyType.Text)
  .Key(x => x.Item.Color, KeyType.Text)
  .CreateAsync().Wait();

// Add Database initialiser 
builder.Services.AddScoped<DatabaseInitialiser>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

// Hook into application lifetime events and trigger only application fully started 
app.Lifetime.ApplicationStarted.Register(async () => {
  // Database Initialiser 
  await app.InitailiseDatabaseAsync();
});

app.MapControllers();

app.Run();
