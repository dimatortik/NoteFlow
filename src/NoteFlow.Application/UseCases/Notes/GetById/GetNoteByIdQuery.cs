using NoteFlow.Application.Messaging;
using NoteFlow.Application.UseCases.Notes.GetAllByUserId;

namespace NoteFlow.Application.UseCases.Notes.GetById;

public record GetNoteByIdQuery(string Id, string UserId) : IQuery<NoteResponse>;
