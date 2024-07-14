using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Azure;
using Sentry.Azure.Functions.Worker;
using System.Data;
using TestFunctionApp;
using Azure.Storage.Blobs;

var host = new HostBuilder()
    .ConfigureAppConfiguration((context, config) =>
    {
        config
              .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();

        var builtConfig = config.Build();
        var azureAppConfigConnectionString = builtConfig.GetConnectionString("AppConfig");

        if (!string.IsNullOrEmpty(azureAppConfigConnectionString))
        {
            config.AddAzureAppConfiguration(azureAppConfigConnectionString);
        }
    })
    .ConfigureFunctionsWorkerDefaults((hostContext, builder) =>
    {
        var configuration = hostContext.Configuration;
        builder.UseSentry(hostContext, options =>
        {
            options.Dsn = configuration["Sentry:Dsn"];
            options.Debug = true;
            options.TracesSampleRate = 1.0;
        });

        // Register the custom middleware
        builder.UseMiddleware<ExceptionHandlingMiddleware>();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddScoped(_ => new SqlConnection(configuration.GetConnectionString("Default")));
        services.InjectAppServices();
        services.AddAzureAppConfiguration();
        // Inject BlobServiceClient as a dependency
        //services.AddSingleton(_ => new BlobServiceClient(configuration["AzureBlobConnectionString"] ?? "This is a test"));
    })
    .Build();

host.Run();
