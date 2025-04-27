using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NoteFlow.Application;

using NoteFlow.Infrastructure.DynamoDB;


namespace NoteFlow.Lambda;

public class LambdaStartup
{
    private static readonly IServiceProvider _serviceProvider;
    private static readonly object _lock = new();

    static LambdaStartup()
    {
        lock (_lock)
        {
            if (_serviceProvider == null)
            {
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                _serviceProvider = serviceCollection.BuildServiceProvider();
            }
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        
        services.AddSingleton<IConfiguration>(configuration);
        
        services.AddLogging(builder =>
        {
            builder.AddLambdaLogger();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        services.AddAwsDynamoDb(configuration);
        
        services.AddApplication(configuration);
        
        services.AddTransient<GraphQlLambdaFunction>();
    }

    public static GraphQlLambdaFunction ResolveLambdaFunction()
    {
        return _serviceProvider.GetRequiredService<GraphQlLambdaFunction>();
    }
}