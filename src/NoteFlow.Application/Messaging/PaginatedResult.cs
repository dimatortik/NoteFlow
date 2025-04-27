namespace NoteFlow.Application.Messaging;

public record PaginatedResult<T>(IEnumerable<T> Items, string? NextToken);

public static class PaginatedResultExtensions
{
    public static PaginatedResult<T> ToPagedResult<T>(this IEnumerable<T> items, string? nextToken)
    {
        return new PaginatedResult<T>(items, nextToken);
    }
}
