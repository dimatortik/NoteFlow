using Amazon.DynamoDBv2.DataModel;
using NoteFlow.Application.Messaging;
using NoteFlow.Domain;
using NoteFlow.Domain.Abstractions;

namespace NoteFlow.Application.UseCases.Notes.Delete;

public class DeleteNoteCommandHandler(DynamoDBContext context) : ICommandHandler<DeleteNoteCommand, string>
{
    public async Task<Result<string>> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        await context.DeleteAsync<Note>(request.UserId,request.Id, cancellationToken);
        return Result.Success(request.Id);
    }
}