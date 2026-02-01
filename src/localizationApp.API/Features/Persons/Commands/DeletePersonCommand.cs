using localizationApp.API.Data;
using MediatR;

namespace localizationApp.API.Features.Persons.Commands;

// Command
public record DeletePersonCommand(int Id) : IRequest<bool>;

// Handler
public class DeletePersonHandler : IRequestHandler<DeletePersonCommand, bool>
{
    private readonly AppDbContext _db;
    private readonly ILogger<DeletePersonHandler> _logger;

    public DeletePersonHandler(AppDbContext db, ILogger<DeletePersonHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<bool> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting person with Id {Id}", request.Id);

        var person = await _db.Persons.FindAsync(new object[] { request.Id }, cancellationToken);

        if (person == null)
        {
            _logger.LogWarning("Person with Id {Id} not found for deletion", request.Id);
            return false;
        }

        _db.Persons.Remove(person);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Person {Id} deleted successfully", request.Id);
        return true;
    }
}