using NoteFlow.Application.Messaging;

namespace NoteFlow.Application.UseCases.Notes.Delete;

public record DeleteNoteCommand(string Id, string UserId) : ICommand<string>;
