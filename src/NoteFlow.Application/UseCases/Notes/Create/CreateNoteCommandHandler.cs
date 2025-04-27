using Amazon.DynamoDBv2.DataModel;
using NoteFlow.Application.Messaging;
using NoteFlow.Application.UseCases.Notes.GetAllByUserId;
using NoteFlow.Domain;
using NoteFlow.Domain.Abstractions;

namespace NoteFlow.Application.UseCases.Notes.Create;

public class CreateNoteCommandHandler(DynamoDBContext context) : ICommandHandler<CreateNoteCommand, NoteResponse>
{
    public async Task<Result<NoteResponse>> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        var note = new Note
        {
            Id = Guid.NewGuid().ToString(),
            Title = request.Title,
            Content = request.Content,
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow,
        };

        await context.SaveAsync(note, cancellationToken: cancellationToken);
        
        var noteResponse = new NoteResponse(
            note.Id,
            note.Title,
            note.Content,
            note.CreatedAt,
            null,
            note.UserId);

        return Result.Success(noteResponse);
    }
}