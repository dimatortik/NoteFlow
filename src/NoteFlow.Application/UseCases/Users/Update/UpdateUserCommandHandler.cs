using Amazon.DynamoDBv2.DataModel;
using NoteFlow.Application.Messaging;
using NoteFlow.Application.UseCases.Users.Get;
using NoteFlow.Domain;
using NoteFlow.Domain.Abstractions;

namespace NoteFlow.Application.UseCases.Users.Update;

public class UpdateUserCommandHandler(DynamoDBContext context) : ICommandHandler<UpdateUserCommand, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var note = new User()
        {
            Id = request.Id,
            Name = request.Name,
            Email = request.Email,
            UpdatedAt = DateTime.UtcNow,
            CreatedAt = null
        };

        await context.SaveAsync(note, new DynamoDBOperationConfig
        {
            SkipVersionCheck = true,
            IgnoreNullValues = true
        },
        cancellationToken: cancellationToken);
        
        var userResponse = new UserResponse(
            note.Id,
            note.Name,
            note.Email,
            note.CreatedAt,
            note.UpdatedAt);
        
        return Result.Success(userResponse);
    }
}