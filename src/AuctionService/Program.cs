using AuctionService.Data;
using AuctionService.Profiles;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(opt => {

    // Add Connection string into NpgSQL
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultDB"));
});
// Add Database Initialiser 
builder.Services.AddScoped<DatabaseInitialiser>();

// Add AutoMapper
var mapperConfig = new MapperConfiguration(x => {
    x.AddProfile(new MappingProfile());
});
var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

var app = builder.Build();

app.UseAuthorization();

// Use Database Initialiser 
app.InitialiserDatabaseAsync().Wait();

app.MapControllers();

app.Run();
