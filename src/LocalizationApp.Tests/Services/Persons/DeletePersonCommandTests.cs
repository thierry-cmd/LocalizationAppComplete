using localizationApp.API.Data;
using localizationApp.API.Features.Persons.Commands;
using localizationApp.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LocalizationApp.Tests.Features.Persons.Commands;

public class DeletePersonCommandTests
{
    private static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static ILogger<DeletePersonHandler> CreateLogger()
    {
        return Substitute.For<ILogger<DeletePersonHandler>>();
    }

    private static async Task<Person> SeedPerson(AppDbContext context, string firstName = "John", string lastName = "Doe")
    {
        var person = new Person
        {
            FirstName = firstName,
            LastName = lastName,
            Email = $"{firstName.ToLower()}@test.com",
            BirthDate = new DateTime(1990, 1, 1),
            Gender = Gender.Male,
            Status = PersonStatus.Active
        };
        context.Persons.Add(person);
        await context.SaveChangesAsync();
        return person;
    }

    [Fact]
    public async Task Handle_ExistingPerson_ReturnsTrue()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var existingPerson = await SeedPerson(context);
        var logger = CreateLogger();
        var handler = new DeletePersonHandler(context, logger);

        var command = new DeletePersonCommand(existingPerson.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ExistingPerson_RemovesFromDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var existingPerson = await SeedPerson(context);
        var personId = existingPerson.Id;
        var logger = CreateLogger();
        var handler = new DeletePersonHandler(context, logger);

        var command = new DeletePersonCommand(personId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedPerson = await context.Persons.FindAsync(personId);
        Assert.Null(deletedPerson);
        Assert.Equal(0, await context.Persons.CountAsync());
    }

    [Fact]
    public async Task Handle_NonExistingPerson_ReturnsFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new DeletePersonHandler(context, logger);

        var command = new DeletePersonCommand(999);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Handle_DeleteOne_OtherPersonsRemain()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var person1 = await SeedPerson(context, "John", "Doe");
        var person2 = await SeedPerson(context, "Jane", "Smith");
        var person3 = await SeedPerson(context, "Bob", "Wilson");
        
        var logger = CreateLogger();
        var handler = new DeletePersonHandler(context, logger);

        var command = new DeletePersonCommand(person2.Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(2, await context.Persons.CountAsync());
        Assert.NotNull(await context.Persons.FindAsync(person1.Id));
        Assert.Null(await context.Persons.FindAsync(person2.Id));
        Assert.NotNull(await context.Persons.FindAsync(person3.Id));
    }

    [Fact]
    public async Task Handle_DeleteSamePerson_Twice_SecondCallReturnsFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var existingPerson = await SeedPerson(context);
        var personId = existingPerson.Id;
        var logger = CreateLogger();
        var handler = new DeletePersonHandler(context, logger);

        var command = new DeletePersonCommand(personId);

        // Act
        var firstResult = await handler.Handle(command, CancellationToken.None);
        var secondResult = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(firstResult);
        Assert.False(secondResult);
    }

    [Fact]
    public async Task Handle_DeleteAllPersons_DatabaseIsEmpty()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var person1 = await SeedPerson(context, "Person", "One");
        var person2 = await SeedPerson(context, "Person", "Two");
        
        var logger = CreateLogger();
        var handler = new DeletePersonHandler(context, logger);

        // Act
        await handler.Handle(new DeletePersonCommand(person1.Id), CancellationToken.None);
        await handler.Handle(new DeletePersonCommand(person2.Id), CancellationToken.None);

        // Assert
        Assert.Equal(0, await context.Persons.CountAsync());
    }
}
