using AutoMapper;
using BindingService.Consumers;
using BindingService.Models;
using BindingService.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.Driver;
using MongoDB.Entities;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch(
    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", 
    true
);

builder.Services.AddControllers();

// Add masstransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("bids", false));
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host => {
            host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
            host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
        });
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        // Tell resource server who the token was effective issued 
        // by and then it can use its configuration
        options.Authority = builder.Configuration["IdentityServiceUrl"];
        // Because IdentityService run on http
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.NameClaimType = "username";
    });


// Add AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add hosted services
builder.Services.AddHostedService<CheckAuctionFinished>();

// Add Grpc client
builder.Services.AddScoped<GrpcAuctionClient>();

// Add MongoDB
DB.InitAsync("BidDb", MongoClientSettings.FromConnectionString(
    builder.Configuration.GetConnectionString("BidDbConnection")))
  .Wait();

DB.Index<Bid>()
  .Key(x => x.AuctionId, KeyType.Text)
  .Key(x => x.Bidder, KeyType.Text)
  .Key(x => x.Status, KeyType.Ascending)
  .CreateAsync()
  .Wait();

var app = builder.Build();

// Add middleware for authentication, authorization 
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

