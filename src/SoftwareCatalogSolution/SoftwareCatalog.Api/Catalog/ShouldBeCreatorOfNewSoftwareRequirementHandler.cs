using Marten;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SoftwareCatalog.Api.Catalog;

public class ShouldBeCreatorOfNewSoftwareRequirement : IAuthorizationRequirement
{
}

public class ShouldBeCreatorOfNewSoftwareRequirementHandler(IDocumentSession session,
    IHttpContextAccessor httpContext) :
    AuthorizationHandler<ShouldBeCreatorOfNewSoftwareRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ShouldBeCreatorOfNewSoftwareRequirement requirement)
    {
        // if the id in the url doesn't map to the person that created this thing, then return forbidden.
        if (httpContext.HttpContext is null) { return; }
        if (httpContext.HttpContext.Request.Method == "DELETE")
        {
            if (httpContext.HttpContext.Request.RouteValues["id"] is string routeParamId)
            {
                if (Guid.TryParse(routeParamId, out Guid itemId))
                {
                    var who = httpContext.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                    var savedEntity = await session.Query<NewSoftwareEntity>().SingleOrDefaultAsync(c => c.Id == itemId);
                    if (savedEntity is null)
                    {
                        return;
                    }
                    else
                    {
                        if (savedEntity.CreatedBy == who)
                        {
                            context.Succeed(requirement);
                        }
                    }
                }
            }
        }
        else
        {
            context.Succeed(requirement);
        }
    }
}
