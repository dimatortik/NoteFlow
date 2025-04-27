using FluentValidation;

namespace NoteFlow.Application.UseCases.Users.Create;

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty()
            .WithMessage("Name is required.")
            .Length(3, 50)
            .WithMessage("Name must be between 3 and 50 characters long.");
        
        RuleFor(x => x.Email).NotNull().NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email must be a valid email address.")
            .Length(5, 100)
            .WithMessage("Email must be between 5 and 100 characters long.");
    }
}