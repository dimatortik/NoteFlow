using Amazon.DynamoDBv2.DataModel;
using NoteFlow.Application.Messaging;
using NoteFlow.Domain;
using NoteFlow.Domain.Abstractions;

namespace NoteFlow.Application.UseCases.Users.Delete;

public class DeleteUserCommandHandler(DynamoDBContext context) : ICommandHandler<DeleteUserCommand, string>
{
    public async Task<Result<string>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var notes = await context.QueryAsync<Note>(request.Id).GetRemainingAsync(cancellationToken);
        
        var batchWrite = context.CreateBatchWrite<Note>();
        foreach (var note in notes)
        {
            await context.DeleteAsync(note, cancellationToken);
        }
        await Task.WhenAll(
            batchWrite.ExecuteAsync(cancellationToken),
            context.DeleteAsync<User>(request.Id, cancellationToken: cancellationToken)
        );
        
        return Result.Success(request.Id);
    }
}