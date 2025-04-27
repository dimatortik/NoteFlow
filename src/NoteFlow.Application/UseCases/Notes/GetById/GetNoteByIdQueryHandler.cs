using Amazon.DynamoDBv2.DataModel;
using NoteFlow.Application.Messaging;
using NoteFlow.Application.UseCases.Notes.GetAllByUserId;
using NoteFlow.Domain;
using NoteFlow.Domain.Abstractions;

namespace NoteFlow.Application.UseCases.Notes.GetById;

public class GetNoteByIdQueryHandler(DynamoDBContext context) : IQueryHandler<GetNoteByIdQuery, NoteResponse>
{
    public async Task<Result<NoteResponse>> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
    {
        var note = await context.LoadAsync<Note>(request.Id, request.UserId, cancellationToken);
        if (note is null)
        {
            return Result.Failure<NoteResponse>(new Error("NoteNotFound", $"Note with Id : {request.Id} not found"));
        }

        var noteResponse = new NoteResponse(
            note.Id,
            note.Title,
            note.Content,
            note.CreatedAt,
            note.UpdatedAt,
            note.UserId);
        
        return Result.Success(noteResponse);

    }
}
