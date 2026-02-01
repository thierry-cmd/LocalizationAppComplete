//using localizationApp.Client.Data;
//using localizationApp.Client.Models;
//using localizationApp.Client.Models.Dtos;
//using localizationApp.Client.Services.Persons;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using NSubstitute;

//namespace LocalizationApp.Tests.Services.Persons;

//public class PersonServiceTests
//{
//    private AppDbContext CreateInMemoryContext()
//    {
//        var options = new DbContextOptionsBuilder<AppDbContext>()
//            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//            .Options;

//        return new AppDbContext(options);
//    }
//    private ILogger<PersonService> CreateLogger()
//    {
//        return Substitute.For<ILogger<PersonService>>();
//    }

//    [Fact]
//    public async Task GetAllAsync_ReturnsEmptyList_WhenNoPersons()
//    {
//        // Arrange
//        using var context = CreateInMemoryContext();
//        var logger = CreateLogger();

//        var service = new PersonService(context, logger);

//        // Act
//        var result = await service.GetAllAsync();

//        // Assert
//        Assert.Empty(result);
//    }

//    [Fact]
//    public async Task GetAllAsync_ReturnsPersons_WhenPersonsExist()
//    {
//        // Arrange
//        using var context = CreateInMemoryContext();
//        context.Persons.Add(new Person
//        {
//            FirstName = "John",
//            LastName = "Doe",
//            Email = "john@test.com"
//        });
//        await context.SaveChangesAsync();
//        var logger = CreateLogger();
//        var service = new PersonService(context, logger);

//        // Act
//        var result = await service.GetAllAsync();

//        // Assert
//        Assert.Single(result);
//        Assert.Equal("John", result[0].FirstName);
//    }

//    [Fact]
//    public async Task CreateAsync_AddsPersonToDatabase()
//    {
//        // Arrange
//        using var context = CreateInMemoryContext();
//        var logger = CreateLogger();
//        var service = new PersonService(context, logger);
//        var dto = new CreatePersonDto
//        {
//            FirstName = "Jane",
//            LastName = "Doe",
//            Email = "jane@test.com"
//        };

//        // Act
//        await service.CreateAsync(dto);

//        // Assert
//        Assert.Single(context.Persons);
//        Assert.Equal("Jane", context.Persons.First().FirstName);
//    }

//    [Fact]
//    public async Task GetByIdAsync_ReturnsPerson_WhenExists()
//    {
//        // Arrange
//        using var context = CreateInMemoryContext();
//        var person = new Person
//        {
//            FirstName = "John",
//            LastName = "Doe",
//            Email = "john@test.com"
//        };
//        context.Persons.Add(person);
//        await context.SaveChangesAsync();
//        var logger = CreateLogger();
//        var service = new PersonService(context, logger);

//        // Act
//        var result = await service.GetByIdAsync(person.Id);

//        // Assert
//        Assert.NotNull(result);
//        Assert.Equal("John", result.FirstName);
//    }

//    [Fact]
//    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
//    {
//        // Arrange
//        using var context = CreateInMemoryContext();
//        var logger = CreateLogger();
//        var service = new PersonService(context, logger);

//        // Act
//        var result = await service.GetByIdAsync(999);

//        // Assert
//        Assert.Null(result);
//    }


//    [Fact]
//    public async Task UpdateAsync_ModifiesPerson_WhenExists()
//    {
//        // Arrange
//        using var context = CreateInMemoryContext();
//        var person = new Person
//        {
//            FirstName = "John",
//            LastName = "Doe",
//            Email = "john@test.com"
//        };
//        context.Persons.Add(person);
//        await context.SaveChangesAsync();

//        var logger = CreateLogger();
//        var service = new PersonService(context, logger);
//        var dto = new UpdatePersonDto
//        {
//            Id = person.Id,
//            FirstName = "Johnny",
//            LastName = "Updated",
//            Email = "johnny@test.com"
//        };

//        // Act
//        await service.UpdateAsync(dto);

//        // Assert
//        var updated = await context.Persons.FindAsync(person.Id);
//        Assert.Equal("Johnny", updated!.FirstName);
//        Assert.Equal("Updated", updated.LastName);
//    }

//    [Fact]
//    public async Task DeleteAsync_RemovesPerson_WhenExists()
//    {
//        // Arrange
//        using var context = CreateInMemoryContext();
//        var person = new Person
//        {
//            FirstName = "John",
//            LastName = "Doe",
//            Email = "john@test.com"
//        };
//        context.Persons.Add(person);
//        await context.SaveChangesAsync();

//        var logger = CreateLogger();
//        var service = new PersonService(context, logger);

//        // Act
//        await service.DeleteAsync(person.Id);

//        // Assert
//        Assert.Empty(context.Persons);
//    }

//    [Fact]
//    public async Task DeleteAsync_DoesNothing_WhenNotExists()
//    {
//        // Arrange
//        using var context = CreateInMemoryContext();
//        var logger = CreateLogger();
//        var service = new PersonService(context, logger);

//        // Act
//        await service.DeleteAsync(999);

//        // Assert
//        Assert.Empty(context.Persons);
//    }
//}
