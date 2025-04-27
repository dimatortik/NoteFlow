using MediatR;
using NoteFlow.Domain.Abstractions;

namespace NoteFlow.Application.Messaging;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}