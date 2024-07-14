using AWS.Logger.SeriLog;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sentry.Azure.Functions.Worker;
using Serilog;
using Serilog.Formatting.Compact;
using TestFunctionApp;

var host = new HostBuilder()
    .ConfigureAppConfiguration((context, config) =>
    {
        config
              .AddJsonFile($"local.settings.json", optional: true, reloadOnChange: true)
              .AddJsonFile("appsettings.json", optional: false)
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

    })
    .UseSerilog((context, services, loggerConfiguration) =>
    {
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration) // Reads what it can from local.settings.json
            .Enrich.FromLogContext()
            .WriteTo.Console(new RenderedCompactJsonFormatter())
            .WriteTo.AWSSeriLog(context.Configuration);
    })
    .Build();

host.Run();
