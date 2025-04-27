using MediatR;
using NoteFlow.Domain.Abstractions;

namespace NoteFlow.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}