using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoteFlow.Application.Behaviors;
using NoteFlow.Application.Messaging;

namespace NoteFlow.Application;

public static class ApplicationDiConfiguration
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
    {
        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssemblyContaining<ICommand>();
            opt.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(ICommand).Assembly);

        return services;
    }
}