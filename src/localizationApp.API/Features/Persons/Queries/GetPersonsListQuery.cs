using localizationApp.API.Data;
using localizationApp.API.Models.Dtos;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace localizationApp.API.Features.Persons.Queries;

// Query
public record GetPersonsListQuery : IRequest<List<PersonListDto>>;

// Handler
public class GetPersonsListHandler : IRequestHandler<GetPersonsListQuery, List<PersonListDto>>
{
    private readonly AppDbContext _db;
    private readonly ILogger<GetPersonsListHandler> _logger;

    public GetPersonsListHandler(AppDbContext db, ILogger<GetPersonsListHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<PersonListDto>> Handle(GetPersonsListQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting persons list (light version)");
        var persons = await _db.Persons.ToListAsync(cancellationToken);
        var result = persons.Adapt<List<PersonListDto>>();
        _logger.LogInformation("Retrieved {Count} persons", result.Count);
        return result;
    }
}