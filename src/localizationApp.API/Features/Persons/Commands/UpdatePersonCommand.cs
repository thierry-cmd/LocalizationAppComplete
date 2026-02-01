using FluentValidation;
using localizationApp.API.Data;
using localizationApp.API.Models;
using localizationApp.API.Models.Dtos;
using Mapster;
using MediatR;

namespace localizationApp.API.Features.Persons.Commands;

// Command
public record UpdatePersonCommand(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    DateTime? BirthDate,
    Gender Gender,
    PersonStatus Status
) : IRequest<PersonDto?>;

// Validator
public class UpdatePersonCommandValidator : AbstractValidator<UpdatePersonCommand>
{
    public UpdatePersonCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Invalid Id");

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
public class UpdatePersonHandler : IRequestHandler<UpdatePersonCommand, PersonDto?>
{
    private readonly AppDbContext _db;
    private readonly ILogger<UpdatePersonHandler> _logger;

    public UpdatePersonHandler(AppDbContext db, ILogger<UpdatePersonHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PersonDto?> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating person with Id {Id}", request.Id);

        var person = await _db.Persons.FindAsync(new object[] { request.Id }, cancellationToken);

        if (person == null)
        {
            _logger.LogWarning("Person with Id {Id} not found for update", request.Id);
            return null;
        }

        person.FirstName = request.FirstName;
        person.LastName = request.LastName;
        person.Email = request.Email;
        person.BirthDate = request.BirthDate;
        person.Gender = request.Gender;
        person.Status = request.Status;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Person {Id} updated successfully", request.Id);
        return person.Adapt<PersonDto>();
    }
}