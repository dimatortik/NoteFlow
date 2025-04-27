using FluentValidation;

namespace NoteFlow.Application.UseCases.Notes.Update;

public class UpdateNoteValidator : AbstractValidator<UpdateNoteCommand>
{
    public UpdateNoteValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .Length(3, 100)
            .WithMessage("Title must be between 3 and 100 characters.");
        
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content is required.")
            .Length(3, 5000)
            .WithMessage("Content must be between 10 and 5000 characters.");

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");
        
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");
    }
}