using NoteFlow.Application.Messaging;
using NoteFlow.Application.UseCases.Notes.GetAllByUserId;
using NoteFlow.Application.UseCases.Users.Get;

namespace NoteFlow.Application.UseCases.Notes.Create;

public record CreateNoteCommand(
    string Title, 
    string Content, 
    string UserId) : ICommand<NoteResponse>;
