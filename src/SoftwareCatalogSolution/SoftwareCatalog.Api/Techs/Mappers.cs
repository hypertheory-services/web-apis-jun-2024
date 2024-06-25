using Riok.Mapperly.Abstractions;

namespace SoftwareCatalog.Api.Techs;


[Mapper]
public static partial class TechMappers
{
    public static partial TechEntity MapToEntity(this TechResponse response);
    public static partial IQueryable<TechResponse> ProjectToResponse(this IQueryable<TechEntity> entity);

    [MapPropertyFromSource(nameof(TechResponse.Id), Use = nameof(NewGuid))]
    public static partial TechResponse MapToResponse(this CreateTechRequest request);

    private static Guid NewGuid(CreateTechRequest _) => Guid.NewGuid();
}

