using localizationApp.API.Data;
using localizationApp.API.Features.Persons.Queries;
using localizationApp.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LocalizationApp.Tests.Features.Persons.Queries;

public class GetAllPersonsQueryTests
{
    private static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static ILogger<GetAllPersonsHandler> CreateLogger()
    {
        return Substitute.For<ILogger<GetAllPersonsHandler>>();
    }

    private static async Task SeedPersons(AppDbContext context, int count)
    {
        for (int i = 1; i <= count; i++)
        {
            context.Persons.Add(new Person
            {
                FirstName = $"Person{i}",
                LastName = $"LastName{i}",
                Email = $"person{i}@test.com",
                BirthDate = new DateTime(1990, 1, i),
                Gender = i % 2 == 0 ? Gender.Female : Gender.Male,
                Status = PersonStatus.Active
            });
        }
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new GetAllPersonsHandler(context, logger);

        var query = new GetAllPersonsQuery();

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
        await SeedPersons(context, 1);
        var logger = CreateLogger();
        var handler = new GetAllPersonsHandler(context, logger);

        var query = new GetAllPersonsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("Person1", result[0].FirstName);
    }

    [Fact]
    public async Task Handle_MultiplePersons_ReturnsAllItems()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        await SeedPersons(context, 5);
        var logger = CreateLogger();
        var handler = new GetAllPersonsHandler(context, logger);

        var query = new GetAllPersonsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectDtoProperties()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com",
            BirthDate = new DateTime(1985, 6, 15),
            Gender = Gender.Male,
            Status = PersonStatus.Pending
        };
        context.Persons.Add(person);
        await context.SaveChangesAsync();

        var logger = CreateLogger();
        var handler = new GetAllPersonsHandler(context, logger);

        var query = new GetAllPersonsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        var dto = result[0];
        Assert.Equal(person.Id, dto.Id);
        Assert.Equal("John", dto.FirstName);
        Assert.Equal("Doe", dto.LastName);
        Assert.Equal("john.doe@test.com", dto.Email);
        Assert.Equal(new DateTime(1985, 6, 15), dto.BirthDate);
        Assert.Equal(Gender.Male, dto.Gender);
        Assert.Equal(PersonStatus.Pending, dto.Status);
    }

    [Fact]
    public async Task Handle_PersonWithNullBirthDate_ReturnsNullInDto()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Persons.Add(new Person
        {
            FirstName = "NoBirthDate",
            LastName = "Person",
            Email = "nobirthdate@test.com",
            BirthDate = null,
            Gender = Gender.Other,
            Status = PersonStatus.Active
        });
        await context.SaveChangesAsync();

        var logger = CreateLogger();
        var handler = new GetAllPersonsHandler(context, logger);

        var query = new GetAllPersonsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Null(result[0].BirthDate);
    }

    [Fact]
    public async Task Handle_AllPersonStatuses_ReturnsCorrectStatus()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Persons.AddRange(
            new Person { FirstName = "Active", LastName = "Person", Email = "a@t.com", Status = PersonStatus.Active },
            new Person { FirstName = "Inactive", LastName = "Person", Email = "i@t.com", Status = PersonStatus.Inactive },
            new Person { FirstName = "Pending", LastName = "Person", Email = "p@t.com", Status = PersonStatus.Pending },
            new Person { FirstName = "Archived", LastName = "Person", Email = "ar@t.com", Status = PersonStatus.Archived }
        );
        await context.SaveChangesAsync();

        var logger = CreateLogger();
        var handler = new GetAllPersonsHandler(context, logger);

        var query = new GetAllPersonsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(4, result.Count);
        Assert.Contains(result, p => p.Status == PersonStatus.Active);
        Assert.Contains(result, p => p.Status == PersonStatus.Inactive);
        Assert.Contains(result, p => p.Status == PersonStatus.Pending);
        Assert.Contains(result, p => p.Status == PersonStatus.Archived);
    }

    [Fact]
    public async Task Handle_AllGenders_ReturnsCorrectGender()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Persons.AddRange(
            new Person { FirstName = "Male", LastName = "Person", Email = "m@t.com", Gender = Gender.Male },
            new Person { FirstName = "Female", LastName = "Person", Email = "f@t.com", Gender = Gender.Female },
            new Person { FirstName = "Other", LastName = "Person", Email = "o@t.com", Gender = Gender.Other }
        );
        await context.SaveChangesAsync();

        var logger = CreateLogger();
        var handler = new GetAllPersonsHandler(context, logger);

        var query = new GetAllPersonsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Contains(result, p => p.Gender == Gender.Male);
        Assert.Contains(result, p => p.Gender == Gender.Female);
        Assert.Contains(result, p => p.Gender == Gender.Other);
    }
}
