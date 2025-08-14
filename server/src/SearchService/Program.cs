using System.Data;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Entities;
using Polly;
using SearchService.Consumers;
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

// Add masstransit
builder.Services.AddMassTransit(x =>
{
  x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
  // Other consumer will be configure by mass transit

  x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

  x.UsingRabbitMq((context, cfg) =>
  {
    cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host => {
      host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest")); 
      host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
    });

    cfg.ReceiveEndpoint("search-auction-created", e =>
    {
      e.UseMessageRetry(r => r.Interval(5, 5));

      e.ConfigureConsumer<AuctionCreatedConsumer>(context);
    });

    cfg.ReceiveEndpoint("search-auction-deleted", e =>
    {
      e.UseMessageRetry(r => r.Immediate(5));

      e.ConfigureConsumer<AuctionDeletedConsumer>(context, c => c.UseMessageRetry(r =>
      {
        r.Interval(10, TimeSpan.FromMilliseconds(200));
        r.Ignore<ArgumentNullException>();
      }));
    });

    cfg.ConfigureEndpoints(context);
  });
});

// Add AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
app.Lifetime.ApplicationStarted.Register(async () =>
{
  // Database Initialiser 
  await Policy.Handle<TimeoutException>()
    .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(5))
    .ExecuteAndCaptureAsync(async () =>
    {
      await app.InitailiseDatabaseAsync();
    });
});

app.MapControllers();

app.Run();
