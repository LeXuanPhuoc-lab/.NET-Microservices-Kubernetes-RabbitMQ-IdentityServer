using IdentityService;
using Polly;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

    var app = builder
        .ConfigureServices()
        .ConfigurePipeline();

    // Hook into application lifetime events and trigger only application fully started 
    // var retryPolicy = Policy.Handle<TimeoutException>()
    //     .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(5))
    //     .ExecuteAndCaptureAsync(async () =>
    //     {
    //         // Ensure database is created here...    
    //         await app.InitialiserDatabaseAsync();
    //     });

    // Hook into application lifetime events and trigger only application fully started 
    app.Lifetime.ApplicationStarted.Register(async () =>
    {
        // Database Initialiser 
        await Policy.Handle<TimeoutException>()
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(5))
            .ExecuteAndCaptureAsync(async () =>
            {
                // this seeding is only for the template to bootstrap the DB and users.
                // in production you will likely want a different approach.
                SeedData.EnsureSeedData(app);

                await Task.CompletedTask;
            });
    });


    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}