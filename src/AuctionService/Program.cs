using AuctionService;
using AuctionService.Configuration;
using AuctionService.Consumers;
using AuctionService.Data;
using AuctionService.Extensions;
using AuctionService.Profiles;
using AuctionService.Services;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Config
builder.Services.Configure<ElasticSettings>(
    builder.Configuration.GetSection("ElasticSettings"));

// Add elastic search
builder.Services.ConfigElastic(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(opt =>
{
    // Add Connection string into NpgSQL
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultDB"));
});
// Add Database Initialiser 
builder.Services.AddScoped<DatabaseInitialiser>();

// Add AutoMapper
var mapperConfig = new MapperConfiguration(x =>
{
    x.AddProfile(new MappingProfile());
});
var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Add masstransit
builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<AuctionDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(10);

        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));

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
    .AddJwtBearer(options =>
    {
        // Tell resource server who the token was effective issued 
        // by and then it can use its configuration
        options.Authority = builder.Configuration["IdentityServiceUrl"];
        // Because IdentityService run on http
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.NameClaimType = "username";
    });

// Add Grpc
builder.Services.AddGrpc();

var app = builder.Build();

// Add middleware for authentication, authorization 
app.UseAuthentication();
app.UseAuthorization();

// Use Database Initialiser 
app.InitialiserDatabaseAsync().Wait();

app.MapControllers();

// Map gRPC service
app.MapGrpcService<GrpcAuctionService>();

app.Run();
