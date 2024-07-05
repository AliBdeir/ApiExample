using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sentry.Azure.Functions.Worker;
using System.Data;
using TestFunctionApp;

var host = new HostBuilder()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
    })
    .ConfigureFunctionsWorkerDefaults((host, builder) =>
    {
        var configuration = host.Configuration;
        builder.UseSentry(host, options =>
        {
            options.Dsn = configuration["Sentry:Dsn"];
            options.Debug = true;
            options.TracesSampleRate = 1.0;
        });
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddScoped(_ => new SqlConnection(configuration.GetConnectionString("Default")));
        services.InjectAppServices();
    })
    .Build();

host.Run();