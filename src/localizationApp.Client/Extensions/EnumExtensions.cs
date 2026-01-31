using localizationApp.Client.Services.Translation;

namespace localizationApp.Client.Extensions;

public static class EnumExtensions
{
    public static string ToLocalizedString<TEnum>(this TEnum enumValue, CategoryLocalizer localizer)
        where TEnum : Enum
    {
        var enumTypeName = typeof(TEnum).Name;

        // Enlever "Person" ou autre préfixe si présent
        var prefix = enumTypeName switch
        {
            "PersonStatus" => "Status",
            "Gender" => "Gender",
            _ => enumTypeName
        };

        var key = $"{prefix}_{enumValue}";
        return localizer[key];
    }
}