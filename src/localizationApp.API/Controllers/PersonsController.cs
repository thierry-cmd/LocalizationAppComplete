using localizationApp.API.Features.Persons.Commands;
using localizationApp.API.Features.Persons.Queries;
using localizationApp.API.Models.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace localizationApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PersonsController> _logger;

    public PersonsController(IMediator mediator, ILogger<PersonsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<PersonDto>>> GetAll()
    {
        _logger.LogInformation("API: GET /api/persons");
        var result = await _mediator.Send(new GetAllPersonsQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PersonDto>> GetById(int id)
    {
        _logger.LogInformation("API: GET /api/persons/{Id}", id);
        var result = await _mediator.Send(new GetPersonByIdQuery(id));

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<PersonDto>> Create(CreatePersonCommand command)
    {
        _logger.LogInformation("API: POST /api/persons");
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PersonDto>> Update(int id, UpdatePersonCommand command)
    {
        _logger.LogInformation("API: PUT /api/persons/{Id}", id);

        if (id != command.Id)
            return BadRequest("Id mismatch");

        var result = await _mediator.Send(command);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        _logger.LogInformation("API: DELETE /api/persons/{Id}", id);
        var result = await _mediator.Send(new DeletePersonCommand(id));

        if (!result)
            return NotFound();

        return NoContent();
    }


    [HttpGet("list")]
    public async Task<ActionResult<List<PersonListDto>>> GetList()
    {
        _logger.LogInformation("API: GET /api/persons/list");
        var result = await _mediator.Send(new GetPersonsListQuery());
        return Ok(result);
    }
}