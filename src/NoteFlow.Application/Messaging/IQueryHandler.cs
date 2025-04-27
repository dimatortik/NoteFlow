using MediatR;
using NoteFlow.Domain.Abstractions;

namespace NoteFlow.Application.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
    
}