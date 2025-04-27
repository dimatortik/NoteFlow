using Amazon.DynamoDBv2.DataModel;
using NoteFlow.Application.Messaging;
using NoteFlow.Domain;
using NoteFlow.Domain.Abstractions;

namespace NoteFlow.Application.UseCases.Users.Get;

public class GetUserQueryHandler(DynamoDBContext context) : IQueryHandler<GetUserQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await context.LoadAsync<User>(request.Id, cancellationToken: cancellationToken);
        
        if (user is null)
        {
            return Result.Failure<UserResponse>(new Error("user_not_found", "User not found"));
        }
        
        var userResponse = new UserResponse(
            user.Id,
            user.Name,
            user.Email,
            user.CreatedAt,
            user.UpdatedAt);
        
        return Result.Success(userResponse);
    }
}