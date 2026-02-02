using FluentValidation.TestHelper;
using localizationApp.API.Data;
using localizationApp.API.Features.Persons.Commands;
using localizationApp.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LocalizationApp.Tests.Features.Persons.Commands;

public class CreatePersonCommandTests
{
    #region Handler Tests

    private static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static ILogger<CreatePersonHandler> CreateLogger()
    {
        return Substitute.For<ILogger<CreatePersonHandler>>();
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesPersonAndReturnsDto()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new CreatePersonHandler(context, logger);

        var command = new CreatePersonCommand(
            FirstName: "John",
            LastName: "Doe",
            Email: "john.doe@test.com",
            BirthDate: new DateTime(1990, 1, 15),
            Gender: Gender.Male,
            Status: PersonStatus.Active
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal("john.doe@test.com", result.Email);
        Assert.Equal(Gender.Male, result.Gender);
        Assert.Equal(PersonStatus.Active, result.Status);
    }

    [Fact]
    public async Task Handle_ValidCommand_PersonIsSavedInDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new CreatePersonHandler(context, logger);

        var command = new CreatePersonCommand(
            FirstName: "Jane",
            LastName: "Smith",
            Email: "jane.smith@test.com",
            BirthDate: null,
            Gender: Gender.Female,
            Status: PersonStatus.Pending
        );

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedPerson = await context.Persons.FirstOrDefaultAsync();
        Assert.NotNull(savedPerson);
        Assert.Equal("Jane", savedPerson.FirstName);
        Assert.Equal("Smith", savedPerson.LastName);
        Assert.Equal("jane.smith@test.com", savedPerson.Email);
    }

    [Fact]
    public async Task Handle_MultipleCommands_CreatesMultiplePersons()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var logger = CreateLogger();
        var handler = new CreatePersonHandler(context, logger);

        var command1 = new CreatePersonCommand("Person", "One", "one@test.com", null, Gender.Male, PersonStatus.Active);
        var command2 = new CreatePersonCommand("Person", "Two", "two@test.com", null, Gender.Female, PersonStatus.Active);

        // Act
        await handler.Handle(command1, CancellationToken.None);
        await handler.Handle(command2, CancellationToken.None);

        // Assert
        Assert.Equal(2, await context.Persons.CountAsync());
    }

    #endregion

    #region Validator Tests

    private readonly CreatePersonCommandValidator _validator = new();

    [Fact]
    public void Validator_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new CreatePersonCommand(
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
    [InlineData("")]
    [InlineData(null)]
    public void Validator_EmptyFirstName_FailsValidation(string? firstName)
    {
        // Arrange
        var command = new CreatePersonCommand(
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
        var command = new CreatePersonCommand(
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
        var command = new CreatePersonCommand(
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
        var command = new CreatePersonCommand(
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
        var command = new CreatePersonCommand(
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
        var command = new CreatePersonCommand(
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
        var command = new CreatePersonCommand(
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
        var command = new CreatePersonCommand(
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
