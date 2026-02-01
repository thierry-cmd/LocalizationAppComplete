using localizationApp.API.Data;
using localizationApp.API.Models.Dtos;
using Mapster;
using MediatR;

namespace localizationApp.API.Features.Persons.Queries;

// Query
public record GetPersonByIdQuery(int Id) : IRequest<PersonDto?>;

// Handler
public class GetPersonByIdHandler : IRequestHandler<GetPersonByIdQuery, PersonDto?>
{
    private readonly AppDbContext _db;
    private readonly ILogger<GetPersonByIdHandler> _logger;

    public GetPersonByIdHandler(AppDbContext db, ILogger<GetPersonByIdHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PersonDto?> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting person with Id {Id}", request.Id);
        var person = await _db.Persons.FindAsync(new object[] { request.Id }, cancellationToken);

        if (person == null)
        {
            _logger.LogWarning("Person with Id {Id} not found", request.Id);
            return null;
        }

        _logger.LogInformation("Retrieved person {FirstName} {LastName}", person.FirstName, person.LastName);
        return person.Adapt<PersonDto>();
    }
}