using JasperFx.Core;
using Marten;
using Microsoft.AspNetCore.Http.HttpResults;
using Riok.Mapperly.Abstractions;
using System.Security.Claims;

namespace SoftwareCatalog.Api.Catalog;


public static class Api
{
    public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder builder)
    {
        var catalogGroup = builder.MapGroup("catalog");
        var newSoftwareGroup = builder.MapGroup("new-software");
        // POST /tech/owned-software
        // POST /techs/b0025560-22da-410b-aaae-fb4ffd1418f9/owned-software
        builder.MapPost("/techs/{techId:guid}/owned-software", TakeOwnershipOfSoftware); // ...
        catalogGroup.MapGet("/", GetTheCatalog);


        newSoftwareGroup.MapPost("/", AddNewSoftwareToCatalog).RequireAuthorization("IsSoftwareCenterAdmin");
        newSoftwareGroup.MapGet("{id:guid}", GetSoftwareById).RequireAuthorization("IsSoftwareCenter");
        newSoftwareGroup.MapGet("/", GetAllNewSoftware).RequireAuthorization("IsSoftwareCenter");
        newSoftwareGroup.MapDelete("{id:guid}", DeleteNewSoftware)
            .RequireAuthorization("IsSoftwareCenterAdmin");
        // TODO We will make up another business rule for this - Only the admin that created this can delete it.
        return builder;
    }

    public static async Task<Results<BadRequest<string>, Ok>> TakeOwnershipOfSoftware(
        Guid techId,
        NewSoftwareResponse request,
        IDocumentSession session,
        CancellationToken token)
    {
        // Rules
        // AuthN/AuthZ - skip.
        // validate the request entity 
        // - If it isn't owned by anyone else, it will be in the table with that same id from the request.
        //  - "It isn't owned by anyone else" - if it's owned by someone else, that won't be in the table
        // what has to happen if those rules are ok
        // - Create a "CatalogEntity" and save it in the database. 
        // - Remove the NewSoftwareEntity from the database (I'll do a hard delete, but you do you)
        // - Set of the reference for the tech so they know they own that software
        // - Set the reference from the CatalogEntity so it knows who the owner is.
        var idOfSoftware = request.Id;
        var savedNewSoftwareEntity = await session
            .Query<NewSoftwareEntity>()
            .SingleOrDefaultAsync(s => s.Id == idOfSoftware, token);

        if (savedNewSoftwareEntity is null)
        {
            return TypedResults.BadRequest("That software no longer exists");
        }
        else
        {
            var newCatalogEntity = new CatalogEntity
            {
                Id = Guid.NewGuid(),
                Title = savedNewSoftwareEntity.Title,
                Description = savedNewSoftwareEntity.Description,
                Owner = techId,

            };
            session.Store(newCatalogEntity);
            session.Delete<NewSoftwareEntity>(savedNewSoftwareEntity.Id);

            await session.SaveChangesAsync(token); // this is a "transaction script"

        }
        return TypedResults.Ok();
    }

    public static async Task<Ok<CollectionResponse<CatalogItemResponse>>> GetTheCatalog(
        IDocumentSession session,
        CancellationToken token)
    {
        var catalog = await session.Query<CatalogEntity>()
            .ToListAsync(token);

        // this is bad code... don't judge (yet)
        List<CatalogItemResponse> data = [];
        foreach (var c in catalog)
        {
            var r = new CatalogItemResponse
            {
                Id = c.Id,
                Title = c.Title,

            };
            r.Embedded.Add("info", new MetaInfo { Description = c.Description });
            r.Links.Add("owner", $"/techs/{c.Owner}");
            data.Add(r);
        }
        return TypedResults.Ok(new CollectionResponse<CatalogItemResponse> { Data = data });
    }

    public static async Task<Created<NewSoftwareResponse>> AddNewSoftwareToCatalog(
        CreateNewSoftwareRequest request,
        TimeProvider clock,
        IHttpContextAccessor contextAcccessor,
        IDocumentSession session
        )
    {
        var who = contextAcccessor?.HttpContext?.User.Claims.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value ?? throw new Exception("Something is wrong with the universe");
        var response = new NewSoftwareResponse
        {
            Id = Guid.NewGuid(),
            AddedOn = clock.GetUtcNow(),
            CreatedBy = who,
            Description = request.Description,
            Title = request.Title
        };


        var entity = response.MapToEntity();
        session.Store(entity);
        await session.SaveChangesAsync();

        return TypedResults.Created($"/new-software/{response.Id}", response);
    }

    public static async Task<Results<Ok<NewSoftwareResponse>, NotFound>> GetSoftwareById(
        Guid id,
        IDocumentSession session
        )
    {
        var entity = await session.Query<NewSoftwareEntity>().SingleOrDefaultAsync(c => c.Id == id);
        return entity switch
        {
            null => TypedResults.NotFound(),
            _ => TypedResults.Ok(entity.MapToResponse())
        };
    }

    public static async Task<Ok<CollectionResponse<NewSoftwareResponse>>> GetAllNewSoftware(
        IHttpContextAccessor contextAcccessor,
        IDocumentSession session)
    {
        var software = await session.Query<NewSoftwareEntity>()
            .ProjectToResponse()
            .ToListAsync();

        var response = new CollectionResponse<NewSoftwareResponse>() { Data = [.. software] };

        return TypedResults.Ok(response);
    }

    public static async Task<Results<NoContent, ForbidHttpResult>> DeleteNewSoftware(
        Guid id,
        IDocumentSession session,
        IHttpContextAccessor contextAccessor)
    {


        session.Delete<NewSoftwareEntity>(id);
        await session.SaveChangesAsync();

        return TypedResults.NoContent();

    }
}


public class CollectionResponse<T>
{
    public IList<T> Data { get; set; } = [];
}

public record CreateNewSoftwareRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}


public record NewSoftwareResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset AddedOn { get; set; }
}

public class NewSoftwareEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset AddedOn { get; set; }
}

[Mapper]
public static partial class NewSoftwareMappers
{
    public static partial NewSoftwareEntity MapToEntity(this NewSoftwareResponse response);
    public static partial NewSoftwareResponse MapToResponse(this NewSoftwareEntity response);
    public static partial IQueryable<NewSoftwareResponse> ProjectToResponse(this IQueryable<NewSoftwareEntity> entity);
}

public class CatalogEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid Owner { get; set; }
}