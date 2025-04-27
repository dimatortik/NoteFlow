using NoteFlow.Application.Messaging;

namespace NoteFlow.Application.UseCases.Notes.GetAllByUserId;

public record GetAllNotesByUserIdQuery(
    string UserId,
    int PageSize,
    string? ContinuationToken) : IQuery<PaginatedResult<NoteResponse>>;


public record NoteResponse(
    string Id, 
    string Title, 
    string Content, 
    DateTime? CreatedAt, 
    DateTime? UpdatedAt,
    string CreatedById);
