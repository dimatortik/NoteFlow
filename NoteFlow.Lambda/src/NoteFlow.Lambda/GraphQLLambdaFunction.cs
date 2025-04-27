using System.Text.Json;
using Amazon.Lambda.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using NoteFlow.Application.UseCases.Notes.Create;
using NoteFlow.Application.UseCases.Notes.Delete;
using NoteFlow.Application.UseCases.Notes.GetAllByUserId;
using NoteFlow.Application.UseCases.Notes.GetById;
using NoteFlow.Application.UseCases.Notes.Update;
using NoteFlow.Application.UseCases.Users.Create;
using NoteFlow.Application.UseCases.Users.Delete;
using NoteFlow.Application.UseCases.Users.Get;
using NoteFlow.Application.UseCases.Users.Update;
using NoteFlow.Domain.Abstractions;
using NoteFlow.Lambda.Helpers;
using NoteFlow.Lambda.Models;
using NoteFlow.Lambda.Models.Note;
using NoteFlow.Lambda.Models.User;

namespace NoteFlow.Lambda;
public class GraphQlLambdaFunction
{
    private readonly IMediator _mediator;
    private readonly ILogger<GraphQlLambdaFunction> _logger;

    public GraphQlLambdaFunction(IMediator mediator, ILogger<GraphQlLambdaFunction> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<object> FunctionHandler(AppSyncEvent request, ILambdaContext context)
    {
        try
        {
            _logger.LogInformation($"Processing GraphQL {request.Field} operation");

            return request.Field switch
            {
                "getUser" => await HandleGetUser(request),
                "createUser" => await HandleCreateUser(request),
                "updateUser" => await HandleUpdateUser(request),
                "deleteUser" => await HandleDeleteUser(request),

                "getNote" => await HandleGetNote(request),
                "getUserNotes" => await HandleGetUserNotes(request),

                "createNote" => await HandleCreateNote(request),
                "updateNote" => await HandleUpdateNote(request),
                "deleteNote" => await HandleDeleteNote(request),

                _ => throw new NotImplementedException($"Field {request.Field} not implemented")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing GraphQL operation {request.Field}");
            return new GraphQlError
            {
                Message = ex.Message,
                Code = "ServerError",
                Path = request.Field
            };
        }
    }

    private async Task<object> HandleGetUser(AppSyncEvent request)
    {
        var userId = request.Arguments["id"].ToString();
        _logger.LogInformation("Getting user with ID: {UserId}", userId);
        
        if (userId is null)
        {
            return GraphQlError.DeserializationError(request.Field);
        }

        var result = await _mediator.Send(new GetUserQuery(userId));

        if (result.IsFailure)
        {
            return new GetUserResult()
            {
                User = null,
                Errors =
                [
                    new GraphQlError
                    {
                        Message = result.Error.Message,
                        Code = result.Error.Code,
                        Path = request.Field
                    }
                ]
            };
        }

        return new GetUserResult
        {
            User = result.Value!,
            Errors = null
        };
    }

    private async Task<object> HandleCreateUser(AppSyncEvent request)
    {
        var input = Deserializer.DeserializeArgument<CreateUserCommand>(request.Arguments, "input");

        if (input is null)
        {
            return GraphQlError.DeserializationError(request.Field);
        }

        _logger.LogInformation("Creating user with name: {Name}", input.Name);

        var result = await _mediator.Send(input);

        if (result.IsFailure)
        {
            return new CreateUserResult()
            {
                User = null,
                Errors =
                [
                    new GraphQlError
                    {
                        Message = result.Error.Message,
                        Code = result.Error.Code,
                        Path = request.Field
                    }
                ]
            };
        }

        return new CreateUserResult
        {
            User = result.Value!,
            Errors = null
        };
    }

    private async Task<object> HandleUpdateUser(AppSyncEvent request)
    {
        var input = Deserializer.DeserializeArgument<UpdateUserCommand>(request.Arguments, "input");
        
        if (input is null)
        {
            return GraphQlError.DeserializationError(request.Field);
        }

        var result = await _mediator.Send(input);

        if (result.IsFailure)
        {
            return new UpdateUserResult()
            {
                User = null,
                Errors =
                [
                    new GraphQlError
                    {
                        Message = result.Error.Message,
                        Code = result.Error.Code,
                        Path = request.Field
                    }
                ]
            };
        }

        return new UpdateUserResult
        {
            User = result.Value!,
            Errors = null
        };
    }

    private async Task<object> HandleDeleteUser(AppSyncEvent request)
    {
        var userId = request.Arguments["id"].ToString();
        
        if (userId is null)
        {
            return GraphQlError.DeserializationError(request.Field);
        }

        var result = await _mediator.Send(new DeleteUserCommand(userId));

        if (result.IsFailure)
        {
            return new DeleteUserResult()
            {
                UserId = null,
                Errors =
                [
                    new GraphQlError
                    {
                        Message = result.Error.Message,
                        Code = result.Error.Code,
                        Path = request.Field
                    }
                ]
            };
        }

        return new DeleteUserResult
        {
            UserId = result.Value!,
            Errors = null
        };
    }

    private async Task<object> HandleGetNote(AppSyncEvent request)
    {
        var noteId = request.Arguments["id"].ToString();
        var userId = request.Arguments["userId"].ToString();

        if (noteId == null || userId == null)
        {
            return GraphQlError.DeserializationError(request.Field);
        }

        var result = await _mediator.Send(new GetNoteByIdQuery(noteId, userId));

        if (result.IsFailure)
        {
            return new GetNoteResult()
            {
                Note = null,
                Errors =
                [
                    new GraphQlError
                    {
                        Message = result.Error.Message,
                        Code = result.Error.Code,
                        Path = request.Field,
                    }
                ]
            };
        }

        return new GetNoteResult
        {
            Note = result.Value!,
            Errors = null
        };
    }

    private async Task<object> HandleGetUserNotes(AppSyncEvent request)
    {
        var userId = request.Arguments["userId"].ToString();

        if (userId == null)
        {
            return new GetUserNotesResult()
            {
                Notes = null,
                Errors =
                [
                    GraphQlError.DeserializationError(request.Field)
                ]
            };
        }
        int pageSize = 3;
        string? continuationToken = null;

        if (request.Arguments.TryGetValue("pageSize", out var argument))
        { 
            if (argument is JsonElement pageSizeElement && pageSizeElement.ValueKind == JsonValueKind.Number)
            {
                pageSize = pageSizeElement.GetInt32();
            }
        }

        if (request.Arguments.TryGetValue("continuationToken", out var requestArgument))
        {
            if (requestArgument is JsonElement tokenElement && tokenElement.ValueKind == JsonValueKind.String)
            {
                continuationToken = tokenElement.GetString();
            }
        }

        var result = await _mediator.Send(new GetAllNotesByUserIdQuery(
            userId,
            pageSize,
            continuationToken));

        if (result.IsFailure)
        {
            return new GetUserNotesResult()
            {
                Notes = null,
                Errors =
                [
                    new GraphQlError
                    {
                        Message = result.Error.Message,
                        Code = result.Error.Code,
                        Path = request.Field
                    }
                ]
            };
        }

        return new GetUserNotesResult
        {
            Notes = result.Value!,
            Errors = null
        };
    }

    private async Task<object> HandleCreateNote(AppSyncEvent request)
    {
        var input = Deserializer.DeserializeArgument<CreateNoteCommand>(request.Arguments, "input");
        if (input is null)
        {
            return GraphQlError.DeserializationError(request.Field);
        }

        var result = await _mediator.Send(input);

        if (result.IsFailure)
        {
            return new CreateNoteResult()
            {
                Note = null,
                Errors =
                [
                    new GraphQlError
                    {
                        Message = result.Error.Message,
                        Code = result.Error.Code,
                        Path = request.Field
                    }
                ]
            };
        }

        return new CreateNoteResult
        {
            Note = result.Value!,
            Errors = null
        };
    }

    private async Task<object> HandleUpdateNote(AppSyncEvent request)
    {
        var input = Deserializer.DeserializeArgument<UpdateNoteCommand>(request.Arguments, "input");

        if (input is null)
        {
            return GraphQlError.DeserializationError(request.Field);
        }

        var result = await _mediator.Send(input);

        if (result.IsFailure)
        {
            return new UpdateNoteResult()
            {
                Note = null,
                Errors =
                [
                    new GraphQlError
                    {
                        Message = result.Error.Message,
                        Code = result.Error.Code,
                        Path = request.Field
                    }
                ]
            };
        }

        return new UpdateNoteResult
        {
            Note = result.Value!,
            Errors = null
        };
    }

    private async Task<object> HandleDeleteNote(AppSyncEvent request)
    {
        var noteId = request.Arguments["id"].ToString();
        var userId = request.Arguments["userId"].ToString();

        if (noteId == null || userId == null)
        {
            return GraphQlError.DeserializationError(request.Field);
        }

        var result = await _mediator.Send(new DeleteNoteCommand(noteId, userId));

        if (result.IsFailure)
        {

            return new DeleteNoteResult()
            {
                NoteId = null,
                Errors =
                [
                    new GraphQlError
                    {
                        Message = result.Error.Message,
                        Code = result.Error.Code,
                        Path = request.Field
                    }
                ]
            };
        }

        return new DeleteNoteResult
        {
            NoteId = noteId,
            Errors = null
        };
    }
}

