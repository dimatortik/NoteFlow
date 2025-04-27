using NoteFlow.Application.Messaging;
using NoteFlow.Application.UseCases.Notes.GetAllByUserId;

namespace NoteFlow.Application.UseCases.Notes.Update;

public record UpdateNoteCommand(string Id, string Title, string Content, string UserId) : ICommand<NoteResponse>;
