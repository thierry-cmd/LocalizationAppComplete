namespace localizationApp.Client.Services.Translation;

public class GlobalTranslations
{
    public CategoryLocalizer Actions { get; }
    public CategoryLocalizer Navigation { get; }
    public CategoryLocalizer Messages { get; }
    public CategoryLocalizer Validation { get; }
    public CategoryLocalizer Home { get; }

    public GlobalTranslations(JsonTranslationLoader loader)
    {
        Actions = new CategoryLocalizer(loader, "Global.Actions");
        Navigation = new CategoryLocalizer(loader, "Global.Navigation");
        Messages = new CategoryLocalizer(loader, "Global.Messages");
        Validation = new CategoryLocalizer(loader, "Global.Validation");
        Home = new CategoryLocalizer(loader, "Global.Home");
    }
}

public class PersonTranslations
{
    public CategoryLocalizer Labels { get; }
    public CategoryLocalizer Titles { get; }
    public CategoryLocalizer Messages { get; }
    public CategoryLocalizer Enums { get; }

    public PersonTranslations(JsonTranslationLoader loader)
    {
        Labels = new CategoryLocalizer(loader, "Person.Labels");
        Titles = new CategoryLocalizer(loader, "Person.Titles");
        Messages = new CategoryLocalizer(loader, "Person.Messages");
        Enums = new CategoryLocalizer(loader, "Person.Enums");
    }
}