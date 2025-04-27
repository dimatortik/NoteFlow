using System.Text.Json;
using System.Text.Json.Nodes;

namespace NoteFlow.Lambda.Helpers;

public static class Deserializer
{
    public static T? DeserializeArgument<T>(Dictionary<string, object> arguments, string key)
    {
        try
        {
            if (arguments[key] is JsonElement jsonElement)
            {
                return jsonElement.Deserialize<T>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else if (arguments[key] is JsonNode jsonNode)
            {
                return JsonSerializer.Deserialize<T>(jsonNode.ToJsonString(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else
            {
                return JsonSerializer.Deserialize<T>(
                    JsonSerializer.Serialize(arguments[key]), 
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid format for {key}", ex);
        }
    }
}