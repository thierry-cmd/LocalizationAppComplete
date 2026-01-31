using localizationApp.Client.Models.Dtos;
using localizationApp.Client.Services.Translation;
using localizationApp.Client.Validators;
using Microsoft.AspNetCore.Hosting;
using NSubstitute;

namespace localizationApp.Tests.Validators;

public class CreatePersonDtoValidatorTests
{
    private readonly CreatePersonDtoValidator _validator;

    public CreatePersonDtoValidatorTests()
    {
        // Créer un faux IWebHostEnvironment qui pointe vers les vrais fichiers
        var env = Substitute.For<IWebHostEnvironment>();
        env.ContentRootPath.Returns(GetProjectPath());

        var loader = new JsonTranslationLoader(env);
        var trad = new TranslationService(loader);

        _validator = new CreatePersonDtoValidator(trad);
    }

    private static string GetProjectPath()
    {
        // Remonte jusqu'au dossier du projet Client
        var currentDir = Directory.GetCurrentDirectory();
        var projectDir = Directory.GetParent(currentDir)?.Parent?.Parent?.Parent?.FullName;
        return Path.Combine(projectDir!, "localizationApp.Client");
    }

    [Fact]
    public async Task FirstName_Empty_ShouldHaveError()
    {
        // Arrange
        var dto = new CreatePersonDto { FirstName = "", LastName = "Doe", Email = "test@test.com" };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
    }
}