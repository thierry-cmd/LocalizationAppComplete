using localizationApp.API.Data;
using localizationApp.API.Features.Persons.Queries;
using localizationApp.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LocalizationApp.Tests.Features.Persons.Queries;

public class GetPersonByIdQueryTests
{
    private static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static ILogger<GetPersonByIdHandler> CreateLogger()
    {
        return Substitute.For<ILogger<GetPersonByIdHandler>>();
    }

    private static async Task<Person> SeedPerson(AppDbContext context)
    {
        var person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com",
            BirthDate = new DateTime(1990, 5, 15),
            Gender = Gender.Male,
            Status = PersonStatus.Active
        };
        context.Persons.Add(person);
        await context.SaveChangesAsync();
        return person;
    }

    [Fact]
    public async Task Handle_ExistingPerson_ReturnsPersonDto()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var existingPerson = await SeedPerson(context);
        var logger = CreateLogger();
        var handler = new GetPersonByIdHandler(context, logger);

        var query = new GetPersonByIdQuery(existingPerson.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingPerson.Id, result.Id);
    }

    [Fact]
    public async Task Handle_ExistingPerson_ReturnsCorrectProperties()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var person = new Person
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@test.com",
            BirthDate = new DateTime(1985, 12, 25),
            Gender = Gender.Female,
            Status = PersonStatus.Pending
        };
        context.Persons.Add(person);
        await context.SaveChangesAsync();

        var logger = CreateLogger();
        var handler = new GetPersonByIdHandler(context, logger);

        var query = new GetPersonByIdQuery(person.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Jane", result.FirstName);
        Assert.Equal("Smith", result.LastName);
        Assert.Equal("jane.smith@test.com", result.Email);
        Assert.Equal(new DateTime(1985, 12, 25), result.BirthDate);
        Assert.Equal(Gender.Female, result.Gender);
        Assert.Equal(PersonStatus.Pending, result.Status);
    }

    [Fact]
    public async Task Handle_NonExistingPerson_ReturnsNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new GetPersonByIdHandler(context, logger);

        var query = new GetPersonByIdQuery(999);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_EmptyDatabase_ReturnsNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new GetPersonByIdHandler(context, logger);

        var query = new GetPersonByIdQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_MultiplePersons_ReturnsCorrectOne()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var person1 = new Person { FirstName = "First", LastName = "Person", Email = "first@test.com" };
        var person2 = new Person { FirstName = "Second", LastName = "Person", Email = "second@test.com" };
        var person3 = new Person { FirstName = "Third", LastName = "Person", Email = "third@test.com" };
        
        context.Persons.AddRange(person1, person2, person3);
        await context.SaveChangesAsync();

        var logger = CreateLogger();
        var handler = new GetPersonByIdHandler(context, logger);

        var query = new GetPersonByIdQuery(person2.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Second", result.FirstName);
        Assert.Equal(person2.Id, result.Id);
    }

    [Fact]
    public async Task Handle_PersonWithNullBirthDate_ReturnsNullBirthDate()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var person = new Person
        {
            FirstName = "No",
            LastName = "BirthDate",
            Email = "nobirth@test.com",
            BirthDate = null,
            Gender = Gender.Other,
            Status = PersonStatus.Inactive
        };
        context.Persons.Add(person);
        await context.SaveChangesAsync();

        var logger = CreateLogger();
        var handler = new GetPersonByIdHandler(context, logger);

        var query = new GetPersonByIdQuery(person.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.BirthDate);
    }

    [Theory]
    [InlineData(PersonStatus.Active)]
    [InlineData(PersonStatus.Inactive)]
    [InlineData(PersonStatus.Pending)]
    [InlineData(PersonStatus.Archived)]
    public async Task Handle_AllStatuses_ReturnsCorrectStatus(PersonStatus status)
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var person = new Person
        {
            FirstName = "Status",
            LastName = "Test",
            Email = "status@test.com",
            Status = status
        };
        context.Persons.Add(person);
        await context.SaveChangesAsync();

        var logger = CreateLogger();
        var handler = new GetPersonByIdHandler(context, logger);

        var query = new GetPersonByIdQuery(person.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(status, result.Status);
    }

    [Theory]
    [InlineData(Gender.Male)]
    [InlineData(Gender.Female)]
    [InlineData(Gender.Other)]
    public async Task Handle_AllGenders_ReturnsCorrectGender(Gender gender)
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var person = new Person
        {
            FirstName = "Gender",
            LastName = "Test",
            Email = "gender@test.com",
            Gender = gender
        };
        context.Persons.Add(person);
        await context.SaveChangesAsync();

        var logger = CreateLogger();
        var handler = new GetPersonByIdHandler(context, logger);

        var query = new GetPersonByIdQuery(person.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gender, result.Gender);
    }
}
