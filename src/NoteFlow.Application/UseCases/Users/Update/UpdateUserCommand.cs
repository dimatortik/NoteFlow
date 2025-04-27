using NoteFlow.Application.Messaging;
using NoteFlow.Application.UseCases.Users.Get;

namespace NoteFlow.Application.UseCases.Users.Update;

public record UpdateUserCommand(
    string Id,
    string Name,
    string Email) : ICommand<UserResponse>;
