using NoteFlow.Application.Messaging;
using NoteFlow.Application.UseCases.Users.Get;


namespace NoteFlow.Application.UseCases.Users.Create;

public record CreateUserCommand(string Name, string Email) : ICommand<UserResponse>;
