using Amazon.DynamoDBv2.DataModel;
using NoteFlow.Application.Messaging;
using NoteFlow.Domain;
using NoteFlow.Domain.Abstractions;

namespace NoteFlow.Application.UseCases.Users.Delete;

public class DeleteUserCommandHandler(DynamoDBContext context) : ICommandHandler<DeleteUserCommand, string>
{
    public async Task<Result<string>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        await context.DeleteAsync<User>(request.Id, cancellationToken);
        return Result.Success(request.Id);
    }
}