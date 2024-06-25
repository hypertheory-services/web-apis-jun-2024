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
        catalogGroup.MapGet("/", GetTheCatalog);


        newSoftwareGroup.MapPost("/", AddNewSoftwareToCatalog).RequireAuthorization("IsSoftwareCenterAdmin");
        newSoftwareGroup.MapGet("{id:guid}", GetSoftwareById).RequireAuthorization("IsSoftwareCenter");
        return builder;
    }

    public static async Task<Ok<string>> GetTheCatalog(
        IDocumentSession session,
        CancellationToken token)
    {
        return TypedResults.Ok("The Catalog Goes Here Minimal");
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
}