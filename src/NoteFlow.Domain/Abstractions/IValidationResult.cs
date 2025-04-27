namespace NoteFlow.Domain.Abstractions;

public interface IValidationResult
{
    public static readonly Error ValidationError = new(
        "validation_error",
        "A validation problem occurred.");

    List<Error> Errors { get; }
}