using localizationApp.Client.Models.Dtos;

namespace localizationApp.Client.Services.Persons;

public interface IPersonService
{
    Task<List<PersonDto>> GetAllAsync();
    Task<PersonDto?> GetByIdAsync(int id);
    Task CreateAsync(CreatePersonDto dto);
    Task UpdateAsync(UpdatePersonDto dto);
    Task DeleteAsync(int id);
}