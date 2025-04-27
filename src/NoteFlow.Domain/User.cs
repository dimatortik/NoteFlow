using Amazon.DynamoDBv2.DataModel;

namespace NoteFlow.Domain;
[DynamoDBTable("users")]
public class User
{
    [DynamoDBHashKey("pk")]
    public string Pk { get; set; } 

    public string Id
    {
        get => Pk; 
        set => Pk = value;
    }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public void Update(string name, string email)
    {
        Name = name;
        Email = email;
        UpdatedAt = DateTime.UtcNow;
    }
}