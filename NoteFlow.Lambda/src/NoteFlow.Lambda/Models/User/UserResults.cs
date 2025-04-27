using NoteFlow.Application.UseCases.Users.Get;

namespace NoteFlow.Lambda.Models.User;

public class CreateUserResult : BaseResult
{
    public required UserResponse? User { get; set; }
}

public class UpdateUserResult : BaseResult
{
    public required UserResponse? User { get; set; }
}
public class GetUserResult : BaseResult
{
    public required UserResponse? User { get; set; }
}
public class DeleteUserResult : BaseResult
{
    public required string? UserId { get; set; }
}

