using FluentValidation;

namespace NoteFlow.Application.UseCases.Users.Update;

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty()
            .WithMessage("Name is required.")
            .Length(3, 50)
            .WithMessage("Name must be between 3 and 50 characters long.");

        RuleFor(x => x.Email).NotNull().NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email must be a valid email address.");

    }
}