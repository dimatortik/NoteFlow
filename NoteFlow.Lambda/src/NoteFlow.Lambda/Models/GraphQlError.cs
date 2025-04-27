namespace NoteFlow.Lambda.Models;

public class GraphQlError
{
    public string Message { get; set; }
    public string Code { get; set; }
    public string Path { get; set; }
    
    public static GraphQlError DeserializationError(string path)
    {
        return new GraphQlError
        {
            Message = "Failed to deserialize the request body.",
            Code = "DeserializationError",
            Path = path,
        };
    }
}