using NoteFlow.Application.Messaging;


namespace NoteFlow.Application.UseCases.Users.Get;

public record GetUserQuery(string Id) : IQuery<UserResponse>;

public record UserResponse(
    string Id,
    string Name,
    string Email,
    DateTime? CreatedAt,
    DateTime? UpdatedAt);
