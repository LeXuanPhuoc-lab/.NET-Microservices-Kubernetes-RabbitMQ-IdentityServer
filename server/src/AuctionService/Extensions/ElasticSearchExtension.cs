using Nest;
using Elasticsearch.Net;
using AuctionService.Repos;


namespace AuctionService.Extensions;
public static class ElasticSearchExtension {
    public static IServiceCollection ConfigElastic(this IServiceCollection services, IConfiguration config)
    {
        // Get elastic settings 
        var elasticSettings = config.GetSection("ElasticSettings");

        // Check exist elastic settings
        if(elasticSettings != null){
            Console.WriteLine("--> Starting config elastic search.");
            
            var settings = new ConnectionSettings(new Uri(elasticSettings["Url"] ?? string.Empty))
                // .BasicAuthentication(credentials["user"], credentials["pass"])
                .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
                .DefaultIndex(elasticSettings["DefaultIndex"])
                .DisablePing(true);

            services.AddSingleton<IElasticClient>(new ElasticClient(settings));
            services.AddScoped(typeof(IElasticGenericRepo<>), typeof(ElasticGenericRepo<>));

            Console.WriteLine("--> Elastic search config succesfully.");
        }    
        

        return services;
    }
}

