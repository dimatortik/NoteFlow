using Amazon.Lambda.Core;
using NoteFlow.Lambda.Models;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.CamelCaseLambdaJsonSerializer))]

namespace NoteFlow.Lambda;

public class GraphQlHandler
{
    public static async Task<object> Handle(AppSyncEvent request, ILambdaContext context)
    {
        var function = LambdaStartup.ResolveLambdaFunction();
        
        return await function.FunctionHandler(request, context);
    }
}