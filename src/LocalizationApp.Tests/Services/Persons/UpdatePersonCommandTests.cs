using FluentValidation.TestHelper;
using localizationApp.API.Data;
using localizationApp.API.Features.Persons.Commands;
using localizationApp.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LocalizationApp.Tests.Features.Persons.Commands;

public class UpdatePersonCommandTests
{
    #region Handler Tests

    private static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static ILogger<UpdatePersonHandler> CreateLogger()
    {
        return Substitute.For<ILogger<UpdatePersonHandler>>();
    }

    private static async Task<Person> SeedPerson(AppDbContext context)
    {
        var person = new Person
        {
            FirstName = "Original",
            LastName = "Name",
            Email = "original@test.com",
            BirthDate = new DateTime(1990, 1, 1),
            Gender = Gender.Male,
            Status = PersonStatus.Pending
        };
        context.Persons.Add(person);
        await context.SaveChangesAsync();
        return person;
    }

    [Fact]
    public async Task Handle_ExistingPerson_UpdatesAndReturnsDto()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var existingPerson = await SeedPerson(context);
        var logger = CreateLogger();
        var handler = new UpdatePersonHandler(context, logger);

        var command = new UpdatePersonCommand(
            Id: existingPerson.Id,
            FirstName: "Updated",
            LastName: "Person",
            Email: "updated@test.com",
            BirthDate: new DateTime(1985, 6, 15),
            Gender: Gender.Female,
            Status: PersonStatus.Active
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingPerson.Id, result.Id);
        Assert.Equal("Updated", result.FirstName);
        Assert.Equal("Person", result.LastName);
        Assert.Equal("updated@test.com", result.Email);
        Assert.Equal(Gender.Female, result.Gender);
        Assert.Equal(PersonStatus.Active, result.Status);
    }

    [Fact]
    public async Task Handle_ExistingPerson_ChangesArePersisted()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var existingPerson = await SeedPerson(context);
        var logger = CreateLogger();
        var handler = new UpdatePersonHandler(context, logger);

        var command = new UpdatePersonCommand(
            Id: existingPerson.Id,
            FirstName: "Persisted",
            LastName: "Changes",
            Email: "persisted@test.com",
            BirthDate: new DateTime(1995, 3, 20),
            Gender: Gender.Other,
            Status: PersonStatus.Inactive
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedPerson = await context.Persons.FindAsync(existingPerson.Id);
        Assert.NotNull(updatedPerson);
        Assert.Equal("Persisted", updatedPerson.FirstName);
        Assert.Equal("Changes", updatedPerson.LastName);
        Assert.Equal("persisted@test.com", updatedPerson.Email);
        Assert.Equal(Gender.Other, updatedPerson.Gender);
        Assert.Equal(PersonStatus.Inactive, updatedPerson.Status);
    }

    [Fact]
    public async Task Handle_NonExistingPerson_ReturnsNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new UpdatePersonHandler(context, logger);

        var command = new UpdatePersonCommand(
            Id: 999,
            FirstName: "Ghost",
            LastName: "Person",
            Email: "ghost@test.com",
            BirthDate: null,
            Gender: Gender.Male,
            Status: PersonStatus.Active
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_PartialUpdate_OnlyUpdatesProvidedFields()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var existingPerson = await SeedPerson(context);
        var logger = CreateLogger();
        var handler = new UpdatePersonHandler(context, logger);

        var command = new UpdatePersonCommand(
            Id: existingPerson.Id,
            FirstName: "OnlyFirstName",
            LastName: existingPerson.LastName,
            Email: existingPerson.Email,
            BirthDate: existingPerson.BirthDate,
            Gender: existingPerson.Gender,
            Status: existingPerson.Status
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("OnlyFirstName", result.FirstName);
        Assert.Equal("Name", result.LastName); // Unchanged
    }

    #endregion

    #region Validator Tests

    private readonly UpdatePersonCommandValidator _validator = new();

    [Fact]
    public void Validator_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new UpdatePersonCommand(
            Id: 1,
            FirstName: "John",
            LastName: "Doe",
            Email: "john@test.com",
            BirthDate: new DateTime(1990, 1, 1),
            Gender: Gender.Male,
            Status: PersonStatus.Active
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validator_InvalidId_FailsValidation(int id)
    {
        // Arrange
        var command = new UpdatePersonCommand(
            Id: id,
            FirstName: "John",
            LastName: "Doe",
            Email: "john@test.com",
            BirthDate: null,
            Gender: Gender.Male,
            Status: PersonStatus.Active
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Invalid Id");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validator_EmptyFirstName_FailsValidation(string? firstName)
    {
        // Arrange
        var command = new UpdatePersonCommand(
            Id: 1,
            FirstName: firstName!,
            LastName: "Doe",
            Email: "john@test.com",
            BirthDate: null,
            Gender: Gender.Male,
            Status: PersonStatus.Active
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name is required");
    }

    [Fact]
    public void Validator_FirstNameTooLong_FailsValidation()
    {
        // Arrange
        var command = new UpdatePersonCommand(
            Id: 1,
            FirstName: new string('A', 51),
            LastName: "Doe",
            Email: "john@test.com",
            BirthDate: null,
            Gender: Gender.Male,
            Status: PersonStatus.Active
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name cannot exceed 50 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validator_EmptyLastName_FailsValidation(string? lastName)
    {
        // Arrange
        var command = new UpdatePersonCommand(
            Id: 1,
            FirstName: "John",
            LastName: lastName!,
            Email: "john@test.com",
            BirthDate: null,
            Gender: Gender.Male,
            Status: PersonStatus.Active
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name is required");
    }

    [Fact]
    public void Validator_LastNameTooLong_FailsValidation()
    {
        // Arrange
        var command = new UpdatePersonCommand(
            Id: 1,
            FirstName: "John",
            LastName: new string('B', 51),
            Email: "john@test.com",
            BirthDate: null,
            Gender: Gender.Male,
            Status: PersonStatus.Active
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name cannot exceed 50 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validator_EmptyEmail_FailsValidation(string? email)
    {
        // Arrange
        var command = new UpdatePersonCommand(
            Id: 1,
            FirstName: "John",
            LastName: "Doe",
            Email: email!,
            BirthDate: null,
            Gender: Gender.Male,
            Status: PersonStatus.Active
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required");
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("invalid@")]
    [InlineData("@test.com")]
    [InlineData("invalid.com")]
    public void Validator_InvalidEmailFormat_FailsValidation(string email)
    {
        // Arrange
        var command = new UpdatePersonCommand(
            Id: 1,
            FirstName: "John",
            LastName: "Doe",
            Email: email,
            BirthDate: null,
            Gender: Gender.Male,
            Status: PersonStatus.Active
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format");
    }

    [Fact]
    public void Validator_FutureBirthDate_FailsValidation()
    {
        // Arrange
        var command = new UpdatePersonCommand(
            Id: 1,
            FirstName: "John",
            LastName: "Doe",
            Email: "john@test.com",
            BirthDate: DateTime.Now.AddDays(1),
            Gender: Gender.Male,
            Status: PersonStatus.Active
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BirthDate)
            .WithErrorMessage("Birth date must be in the past");
    }

    [Fact]
    public void Validator_NullBirthDate_PassesValidation()
    {
        // Arrange
        var command = new UpdatePersonCommand(
            Id: 1,
            FirstName: "John",
            LastName: "Doe",
            Email: "john@test.com",
            BirthDate: null,
            Gender: Gender.Male,
            Status: PersonStatus.Active
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BirthDate);
    }

    #endregion
}
