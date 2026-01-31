using localizationApp.Client.Services.Translation;
using Microsoft.AspNetCore.Hosting;
using NSubstitute;

namespace localizationApp.Tests.Services.Translation;

public class JsonTranslationLoaderTests
{
    private readonly JsonTranslationLoader _loader;

    public JsonTranslationLoaderTests()
    {
        var env = Substitute.For<IWebHostEnvironment>();
        env.ContentRootPath.Returns(GetProjectPath());
        _loader = new JsonTranslationLoader(env);
    }

    private static string GetProjectPath()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var projectDir = Directory.GetParent(currentDir)?.Parent?.Parent?.Parent?.FullName;
        return Path.Combine(projectDir!, "localizationApp.Client");
    }

    [Fact]
    public void Get_ReturnsEnglishTranslation()
    {
        // Act
        var result = _loader.Get("Person.Labels.FirstName", "en");

        // Assert
        Assert.Equal("First Name", result);
    }

    [Fact]
    public void Get_ReturnsFrenchTranslation()
    {
        // Act
        var result = _loader.Get("Person.Labels.FirstName", "fr");

        // Assert
        Assert.Equal("Prénom", result);
    }

    [Fact]
    public void Get_ReturnsDutchTranslation()
    {
        // Act
        var result = _loader.Get("Person.Labels.FirstName", "nl");

        // Assert
        Assert.Equal("Voornaam", result);
    }


    [Fact]
    public void Get_FallbackToEnglish_WhenCultureNotFound()
    {
        // Act - "de" (allemand) n'existe pas
        var result = _loader.Get("Person.Labels.FirstName", "de");

        // Assert - Doit retourner la version anglaise
        Assert.Equal("First Name", result);
    }

    [Fact]
    public void Get_ReturnsKey_WhenKeyNotFound()
    {
        // Act
        var result = _loader.Get("Person.Labels.UnknownKey", "en");

        // Assert - Doit retourner la clé elle-même
        Assert.Equal("Person.Labels.UnknownKey", result);
    }

    [Fact]
    public void Get_ReturnsEnumTranslation()
    {
        // Act
        var result = _loader.Get("Person.Enums.Gender_Male", "fr");

        // Assert
        Assert.Equal("Homme", result);
    }

}