using Marten;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SoftwareCatalog.Api.Catalog;


public static class Api
{
    public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("catalog");
        group.MapGet("/", GetTheCatalog);

        return builder;
    }

    public static async Task<Ok<string>> GetTheCatalog(
        IDocumentSession session,
        CancellationToken token)
    {
        return TypedResults.Ok("The Catalog Goes Here Minimal");
    }

}