using Mapster;
using localizationApp.API.Models;
using localizationApp.API.Models.Dtos;

namespace localizationApp.API.Mapping;

public static class MappingConfig
{
    public static void RegisterMappings()
    {
        // Mapping Person → PersonListDto
        TypeAdapterConfig<Person, PersonListDto>.NewConfig()
            .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
            .Map(dest => dest.Status, src => src.Status.ToString());


        // TypeAdapterConfig<Source, Destination>.NewConfig()
        //     .Map(dest => dest.X, src => src.Y);
    }
}