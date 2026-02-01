using localizationApp.Client.Models.Dtos;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace localizationApp.Client.Services.Persons;

public class PersonService : IPersonService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PersonService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public PersonService(HttpClient httpClient, ILogger<PersonService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public async Task<List<PersonDto>> GetAllAsync()
    {
        _logger.LogInformation("Calling API: GET /api/persons");
        var result = await _httpClient.GetFromJsonAsync<List<PersonDto>>("api/persons", _jsonOptions);
        _logger.LogInformation("Retrieved {Count} persons from API", result?.Count ?? 0);
        return result ?? new List<PersonDto>();
    }

    public async Task<PersonDto?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Calling API: GET /api/persons/{Id}", id);
        try
        {
            return await _httpClient.GetFromJsonAsync<PersonDto>($"api/persons/{id}", _jsonOptions);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Person with Id {Id} not found", id);
            return null;
        }
    }

    public async Task CreateAsync(CreatePersonDto dto)
    {
        _logger.LogInformation("Calling API: POST /api/persons");
        var response = await _httpClient.PostAsJsonAsync("api/persons", dto, _jsonOptions);
        response.EnsureSuccessStatusCode();
        _logger.LogInformation("Person created successfully");
    }

    public async Task UpdateAsync(UpdatePersonDto dto)
    {
        _logger.LogInformation("Calling API: PUT /api/persons/{Id}", dto.Id);
        var response = await _httpClient.PutAsJsonAsync($"api/persons/{dto.Id}", dto, _jsonOptions);
        response.EnsureSuccessStatusCode();
        _logger.LogInformation("Person {Id} updated successfully", dto.Id);
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Calling API: DELETE /api/persons/{Id}", id);
        var response = await _httpClient.DeleteAsync($"api/persons/{id}");
        response.EnsureSuccessStatusCode();
        _logger.LogInformation("Person {Id} deleted successfully", id);
    }
}