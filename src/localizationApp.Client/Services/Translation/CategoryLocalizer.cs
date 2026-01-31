using System.Globalization;

namespace localizationApp.Client.Services.Translation;

public class CategoryLocalizer
{
    private readonly JsonTranslationLoader _loader;
    private readonly string _prefix;

    public CategoryLocalizer(JsonTranslationLoader loader, string prefix)
    {
        _loader = loader;
        _prefix = prefix;
    }

    public string this[string key]
    {
        get
        {
            var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var fullKey = $"{_prefix}.{key}";

            // Debug
            Console.WriteLine($"Getting: {fullKey} for culture: {culture}");

            return _loader.Get(fullKey, culture);
        }
    }
}