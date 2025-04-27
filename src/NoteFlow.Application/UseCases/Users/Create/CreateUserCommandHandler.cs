using Amazon.DynamoDBv2.DataModel;
using NoteFlow.Application.Messaging;
using NoteFlow.Application.UseCases.Users.Get;
using NoteFlow.Domain;
using NoteFlow.Domain.Abstractions;

namespace NoteFlow.Application.UseCases.Users.Create;

public class CreateUserCommandHandler(DynamoDBContext context) : ICommandHandler<CreateUserCommand, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow,
        };

        await context.SaveAsync(user, cancellationToken);
        
        var userResponse = new UserResponse(
            user.Id,
            user.Name,
            user.Email,
            user.CreatedAt,
            DateTime.UtcNow);

        return Result.Success(userResponse);
    }
}