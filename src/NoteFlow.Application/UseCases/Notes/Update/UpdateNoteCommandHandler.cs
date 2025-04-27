using Amazon.DynamoDBv2.DataModel;
using NoteFlow.Application.Messaging;
using NoteFlow.Application.UseCases.Notes.GetAllByUserId;
using NoteFlow.Domain;
using NoteFlow.Domain.Abstractions;

namespace NoteFlow.Application.UseCases.Notes.Update;

public class UpdateNoteCommandHandler(DynamoDBContext context) : ICommandHandler<UpdateNoteCommand, NoteResponse>
{
    public async Task<Result<NoteResponse>> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        var note = new Note
        {
            Id = request.Id,
            Title = request.Title,
            Content = request.Content,
            UpdatedAt = DateTime.UtcNow,
            UserId = request.UserId,
            CreatedAt = null
        };
       
       

        await context.SaveAsync(note, new DynamoDBOperationConfig()
        {
            SkipVersionCheck = true,
            IgnoreNullValues = true
        },
        cancellationToken: cancellationToken);
        
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