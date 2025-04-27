using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using NoteFlow.Application.Messaging;
using NoteFlow.Domain;
using NoteFlow.Domain.Abstractions;

namespace NoteFlow.Application.UseCases.Notes.GetAllByUserId;

public class GetAllNotesByUserIdQueryHandler(DynamoDBContext context) : IQueryHandler<GetAllNotesByUserIdQuery, PaginatedResult<NoteResponse>>
{
    public async Task<Result<PaginatedResult<NoteResponse>>> Handle(
        GetAllNotesByUserIdQuery request, 
        CancellationToken cancellationToken)
    {
        var queryConfig = new QueryOperationConfig
        {
            IndexName = "UserId-CreatedAt-Note-index",
            KeyExpression = new Expression
            {
                ExpressionStatement = "pk= :pk",
                ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
                {
                    [":pk"] = request.UserId
                }
            },
            PaginationToken = request.ContinuationToken,
            Limit = request.PageSize
        };
        
        var noteTable = context.GetTargetTable<Note>();
        
        var search = noteTable.Query(queryConfig);
        
        var page = await search.GetNextSetAsync(cancellationToken);
        var nextToken = search.PaginationToken;
        
        var notes = context.FromDocuments<Note>(page)
            .Select(note => new NoteResponse(
                note.Id,
                note.Title,
                note.Content,
                note.CreatedAt,
                note.UpdatedAt,
                note.UserId))
            .ToList();
            
        var paginatedResult = new PaginatedResult<NoteResponse>(
            notes,
            nextToken
        );

        return Result.Success(paginatedResult);
    }
}