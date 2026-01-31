using System.Text.Json;

namespace localizationApp.Client.Services.Translation;

public class JsonTranslationLoader
{
    private readonly Dictionary<string, Dictionary<string, string>> _translations = new();
    private readonly string _translationsPath;

    public JsonTranslationLoader(IWebHostEnvironment env)
    {
        _translationsPath = Path.Combine(env.ContentRootPath, "Translations");
        LoadAllTranslations();
    }

    private void LoadAllTranslations()
    {
        if (!Directory.Exists(_translationsPath))
        {
            throw new DirectoryNotFoundException($"Translations folder not found: {_translationsPath}");
        }

        var files = Directory.GetFiles(_translationsPath, "*.json");
        Console.WriteLine($"Found {files.Length} translation files:");

        foreach (var file in files)
        {
            Console.WriteLine($"  - {file}");

            var fileName = Path.GetFileNameWithoutExtension(file);
            var parts = fileName.Split('.');


            if (parts.Length != 2)
            {
                throw new Exception($"Invalid translation file name: {fileName}. Expected format: Module.culture.json");
            }

            var module = parts[0];
            var culture = parts[1];

            if (!_translations.ContainsKey(culture))
            {
                _translations[culture] = new Dictionary<string, string>();
            }

            var json = File.ReadAllText(file);
            var categories = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);

            if (categories == null) continue;

            foreach (var category in categories)
            {
                foreach (var key in category.Value)
                {
                    var fullKey = $"{module}.{category.Key}.{key.Key}";

                    if (_translations[culture].ContainsKey(fullKey))
                    {
                        throw new Exception($"Duplicate translation key: {fullKey} in culture {culture}");
                    }

                    _translations[culture][fullKey] = key.Value;
                }
            }
        }

        Console.WriteLine($"Loaded cultures: {string.Join(", ", _translations.Keys)}");
        foreach (var culture in _translations.Keys)
        {
            Console.WriteLine($"  - {culture}: {_translations[culture].Count} keys");
        }
    }

    public string Get(string fullKey, string culture)
    {
        // Essayer la culture demandée
        if (_translations.TryGetValue(culture, out var cultureDict))
        {
            if (cultureDict.TryGetValue(fullKey, out var value))
            {
                return value;
            }
        }

        // Fallback vers anglais
        if (culture != "en" && _translations.TryGetValue("en", out var enDict))
        {
            if (enDict.TryGetValue(fullKey, out var value))
            {
                return value;
            }
        }

        // Retourner la clé si non trouvée
        return fullKey;
    }

    public IEnumerable<string> GetSupportedCultures()
    {
        return _translations.Keys;
    }
}