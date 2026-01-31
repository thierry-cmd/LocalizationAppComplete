namespace localizationApp.Client.Services.Translation;

public class TranslationService
{
    public GlobalTranslations Global { get; }
    public PersonTranslations Person { get; }

    public TranslationService(JsonTranslationLoader loader)
    {
        Global = new GlobalTranslations(loader);
        Person = new PersonTranslations(loader);
    }
}