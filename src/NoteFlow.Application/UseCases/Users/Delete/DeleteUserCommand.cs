using NoteFlow.Application.Messaging;

namespace NoteFlow.Application.UseCases.Users.Delete;

public record DeleteUserCommand(string Id) : ICommand<string>;
