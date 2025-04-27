using Amazon.DynamoDBv2.DataModel;

namespace NoteFlow.Domain;
[DynamoDBTable("notes")]
public class Note
{
    [DynamoDBHashKey(AttributeName = "pk")]
    public string Pk { get; set; }

    [DynamoDBRangeKey(AttributeName = "sk")]
    public string Sk { get; set; }

    public string Id
    {
        get => Sk;
        set => Sk = value;
    }

    public string UserId
    {
        get => Pk;
        set => Pk = value;
    }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public void Update(string title, string content)
    {
        Title = title;
        Content = content;
        UpdatedAt = DateTime.UtcNow;
    }
}