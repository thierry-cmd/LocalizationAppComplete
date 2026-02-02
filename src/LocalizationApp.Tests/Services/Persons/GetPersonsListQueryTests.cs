using localizationApp.API.Data;
using localizationApp.API.Features.Persons.Queries;
using localizationApp.API.Mapping;
using localizationApp.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LocalizationApp.Tests.Features.Persons.Queries;

public class GetPersonsListQueryTests
{
    static GetPersonsListQueryTests()
    {
        // Initialiser la configuration Mapster une seule fois pour tous les tests
        MappingConfig.RegisterMappings();
    }

    private static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static ILogger<GetPersonsListHandler> CreateLogger()
    {
        return Substitute.For<ILogger<GetPersonsListHandler>>();
    }

    [Fact]
    public async Task Handle_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new GetPersonsListHandler(context, logger);

        var query = new GetPersonsListQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_SinglePerson_ReturnsSingleItem()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Persons.Add(new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Status = PersonStatus.Active
        });
        await context.SaveChangesAsync();

        var logger = CreateLogger();
        var handler = new GetPersonsListHandler(context, logger);

        var query = new GetPersonsListQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task Handle_MultiplePersons_ReturnsAllItems()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Persons.AddRange(
            new Person { FirstName = "Person", LastName = "One", Email = "one@test.com" },
            new Person { FirstName = "Person", LastName = "Two", Email = "two@test.com" },
            new Person { FirstName = "Person", LastName = "Three", Email = "three@test.com" }
        );
        await context.SaveChangesAsync();

        var logger = CreateLogger();
        var handler = new GetPersonsListHandler(context, logger);

        var query = new GetPersonsListQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectFullName()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Persons.Add(new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Status = PersonStatus.Active
        });
        await context.SaveChangesAsync();

        var logger = CreateLogger();
        var handler = new GetPersonsListHandler(context, logger);

        var query = new GetPersonsListQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("John Doe", result[0].FullName);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectEmail()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Persons.Add(new Person
        {
            FirstName = "Test",
            LastName = "Person",
            Email = "specific.email@test.com",
            Status = PersonStatus.Active
        });
        await context.SaveChangesAsync();

        var logger = CreateLogger();
        var handler = new GetPersonsListHandler(context, logger);

        var query = new GetPersonsListQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("specific.email@test.com", result[0].Email);
    }

    [Fact]
    public async Task Handle_ReturnsStatusAsString()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Persons.Add(new Person
        {
            FirstName = "Active",
            LastName = "Person",
            Email = "active@test.com",
            Status = PersonStatus.Active
        });
        await context.SaveChangesAsync();

        var logger = CreateLogger();
        var handler = new GetPersonsListHandler(context, logger);

        var query = new GetPersonsListQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("Active", result[0].Status);
    }

    [Theory]
    [InlineData(PersonStatus.Active, "Active")]
    [InlineData(PersonStatus.Inactive, "Inactive")]
    [InlineData(PersonStatus.Pending, "Pending")]
    [InlineData(PersonStatus.Archived, "Archived")]
    public async Task Handle_AllStatuses_ReturnsCorrectStatusString(PersonStatus status, string expectedStatusString)
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Persons.Add(new Person
        {
            FirstName = "Status",
            LastName = "Test",
            Email = "status@test.com",
            Status = status
        });
        await context.SaveChangesAsync();

        var logger = CreateLogger();
        var handler = new GetPersonsListHandler(context, logger);

        var query = new GetPersonsListQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal(expectedStatusString, result[0].Status);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectId()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var person = new Person
        {
            FirstName = "Test",
            LastName = "Person",
            Email = "test@test.com"
        };
        context.Persons.Add(person);
        await context.SaveChangesAsync();

        var logger = CreateLogger();
        var handler = new GetPersonsListHandler(context, logger);

        var query = new GetPersonsListQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal(person.Id, result[0].Id);
    }

    [Fact]
    public async Task Handle_PersonListDto_DoesNotContainBirthDateOrGender()
    {
        // Arrange - PersonListDto is a "light" DTO without BirthDate and Gender
        using var context = CreateInMemoryContext();
        context.Persons.Add(new Person
        {
            FirstName = "Full",
            LastName = "Person",
            Email = "full@test.com",
            BirthDate = new DateTime(1990, 1, 1),
            Gender = Gender.Male,
            Status = PersonStatus.Active
        });
        await context.SaveChangesAsync();

        var logger = CreateLogger();
        var handler = new GetPersonsListHandler(context, logger);

        var query = new GetPersonsListQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        // PersonListDto should only have: Id, FullName, Email, Status
        var dto = result[0];
        Assert.True(dto.Id > 0);
        Assert.Equal("Full Person", dto.FullName);
        Assert.Equal("full@test.com", dto.Email);
        Assert.Equal("Active", dto.Status);
    }

    [Fact]
    public async Task Handle_VariousFullNames_FormatsCorrectly()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Persons.AddRange(
            new Person { FirstName = "Jean-Pierre", LastName = "Dupont", Email = "jp@test.com" },
            new Person { FirstName = "María", LastName = "García López", Email = "maria@test.com" },
            new Person { FirstName = "李", LastName = "明", Email = "li@test.com" }
        );
        await context.SaveChangesAsync();

        var logger = CreateLogger();
        var handler = new GetPersonsListHandler(context, logger);

        var query = new GetPersonsListQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Contains(result, p => p.FullName == "Jean-Pierre Dupont");
        Assert.Contains(result, p => p.FullName == "María García López");
        Assert.Contains(result, p => p.FullName == "李 明");
    }
}