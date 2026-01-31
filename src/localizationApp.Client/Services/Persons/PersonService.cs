using localizationApp.Client.Data;
using localizationApp.Client.Models;
using localizationApp.Client.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace localizationApp.Client.Services.Persons;

public class PersonService : IPersonService
{
    private readonly AppDbContext _db;

    public PersonService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<PersonDto>> GetAllAsync()
    {
        return await _db.Persons
            .Select(p => new PersonDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                BirthDate = p.BirthDate,
                Gender = p.Gender,
                Status = p.Status
            })
            .ToListAsync();
    }

    public async Task<PersonDto?> GetByIdAsync(int id)
    {
        var person = await _db.Persons.FindAsync(id);
        if (person == null) return null;

        return new PersonDto
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            Email = person.Email,
            BirthDate = person.BirthDate,
            Gender = person.Gender,
            Status = person.Status
        };
    }

    public async Task CreateAsync(CreatePersonDto dto)
    {
        var person = new Person
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            BirthDate = dto.BirthDate,
            Gender = dto.Gender,
            Status = dto.Status
        };

        _db.Persons.Add(person);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdatePersonDto dto)
    {
        var person = await _db.Persons.FindAsync(dto.Id);
        if (person == null) return;

        person.FirstName = dto.FirstName;
        person.LastName = dto.LastName;
        person.Email = dto.Email;
        person.BirthDate = dto.BirthDate;
        person.Gender = dto.Gender;
        person.Status = dto.Status;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var person = await _db.Persons.FindAsync(id);
        if (person != null)
        {
            _db.Persons.Remove(person);
            await _db.SaveChangesAsync();
        }
    }
}