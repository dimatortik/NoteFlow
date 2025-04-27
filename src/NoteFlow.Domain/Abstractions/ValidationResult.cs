namespace NoteFlow.Domain.Abstractions;

public sealed class ValidationResult : Result, IValidationResult
{
    private ValidationResult(List<Error> errors)
        : base(false, IValidationResult.ValidationError) =>
        Errors = errors;

    private ValidationResult(Error error)
        : base(false, IValidationResult.ValidationError) =>
        Errors!.Add(error);

    public List<Error> Errors { get; }
    
    public ValidationResult WithError(Error error) => new(error);
    
    public static ValidationResult WithErrors(List<Error> errors) => new(errors);
}
public sealed class ValidationResult<TValue> : Result<TValue>, IValidationResult
{
    private ValidationResult(Error error)
        : base(default, false, IValidationResult.ValidationError) =>
        Errors!.Add(error);
    private ValidationResult(List<Error> errors)
        : base(default, false, IValidationResult.ValidationError) =>
        Errors = errors;

    public List<Error> Errors { get; }
    
    public static ValidationResult<TValue> WithError(Error error) => new(error);

    public static ValidationResult<TValue> WithErrors(List<Error> errors) => new(errors);
}