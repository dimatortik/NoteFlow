using System.Text.Json.Serialization;

namespace NoteFlow.Lambda.Models;

public class AppSyncEvent
{

    [JsonPropertyName("field")] 
    public string Field { get; set; }
    
    [JsonPropertyName("arguments")] 
    public Dictionary<string, object> Arguments { get; set; }
    
    [JsonPropertyName("identity")] 
    public AppSyncIdentity Identity { get; set; }
    
    [JsonPropertyName("source")] 
    public object Source { get; set; }
    
    [JsonPropertyName("info")] 
    public AppSyncInfo Info { get; set; }
}

public class AppSyncIdentity
{
    [JsonPropertyName("username")]
    public string Username { get; set; }
    
    [JsonPropertyName("claims")] 
    public Dictionary<string, string> Claims { get; set; }
    
    [JsonPropertyName("sourceIp")] 
    public string[] SourceIp { get; set; }
    
    [JsonPropertyName("defaultAuthStrategy")] 
    public string DefaultAuthStrategy { get; set; }
}

public class AppSyncInfo
{
    [JsonPropertyName("fieldName")] 
    public string FieldName { get; set; }
    
    [JsonPropertyName("parentTypeName")] 
    public string ParentTypeName { get; set; }
    
    [JsonPropertyName("variables")] 
    public Dictionary<string, object> Variables { get; set; }
    
    [JsonPropertyName("selectionSetList")] 
    public List<string> SelectionSetList { get; set; }
}