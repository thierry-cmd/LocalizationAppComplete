using localizationApp.API.Data;
using localizationApp.API.Models.Dtos;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace localizationApp.API.Features.Persons.Queries;

// Query (la requête)
public record GetAllPersonsQuery : IRequest<List<PersonDto>>;

// Handler (le traitement)
public class GetAllPersonsHandler : IRequestHandler<GetAllPersonsQuery, List<PersonDto>>
{
    private readonly AppDbContext _db;
    private readonly ILogger<GetAllPersonsHandler> _logger;

    public GetAllPersonsHandler(AppDbContext db, ILogger<GetAllPersonsHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<PersonDto>> Handle(GetAllPersonsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all persons");
        var persons = await _db.Persons.ToListAsync(cancellationToken);
        _logger.LogInformation("Retrieved {Count} persons", persons.Count);
        return persons.Adapt<List<PersonDto>>();
    }
}