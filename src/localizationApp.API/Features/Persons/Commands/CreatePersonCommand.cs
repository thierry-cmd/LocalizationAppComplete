using FluentValidation;
using localizationApp.API.Data;
using localizationApp.API.Models;
using localizationApp.API.Models.Dtos;
using Mapster;
using MediatR;

namespace localizationApp.API.Features.Persons.Commands;

// Command
public record CreatePersonCommand(
    string FirstName,
    string LastName,
    string Email,
    DateTime? BirthDate,
    Gender Gender,
    PersonStatus Status
) : IRequest<PersonDto>;

// Validator
public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
{
    public CreatePersonCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Now).WithMessage("Birth date must be in the past")
            .When(x => x.BirthDate.HasValue);
    }
}

// Handler
public class CreatePersonHandler : IRequestHandler<CreatePersonCommand, PersonDto>
{
    private readonly AppDbContext _db;
    private readonly ILogger<CreatePersonHandler> _logger;

    public CreatePersonHandler(AppDbContext db, ILogger<CreatePersonHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PersonDto> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating person {FirstName} {LastName}", request.FirstName, request.LastName);

        var person = new Person
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            BirthDate = request.BirthDate,
            Gender = request.Gender,
            Status = request.Status
        };

        _db.Persons.Add(person);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Person created with Id {Id}", person.Id);
        return person.Adapt<PersonDto>();
    }
}