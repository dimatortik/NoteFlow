using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace NoteFlow.Infrastructure.DynamoDB;

public static class DynamoDbConfiguration
{
    public static IServiceCollection AddAwsDynamoDb(this IServiceCollection services, IConfiguration config)
    {
        var awsOptions = config.GetAWSOptions();
        services.AddDefaultAWSOptions(awsOptions);

        services.AddAWSService<IAmazonDynamoDB>();

        services.AddSingleton<DynamoDBContext>(sp =>
        {
            var client = sp.GetRequiredService<IAmazonDynamoDB>();
            var contextConfig = new DynamoDBContextConfig
            {
                Conversion = DynamoDBEntryConversion.V2,
                SkipVersionCheck = false,
                ConsistentRead = false,
            };
            return new DynamoDBContext(client, contextConfig);
        });

        return services;
    }
}