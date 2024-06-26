

using JasperFx.Core;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace SoftwareCatalog.Api.Techs;

[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Techs")]
[Route("techs")]
public class Api(IDocumentSession session) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Tags = ["Techs"])]
    public async Task<ActionResult<TechResponse>> AddATechAsync(
        [FromBody] CreateTechRequest request,
        [FromServices] IValidator<CreateTechRequest> validator,

        [FromServices] TimeProvider timeProvider,
        CancellationToken token)
    {
        // Judge the heck out of us.
        var validations = await validator.ValidateAsync(request, token);
        if (!validations.IsValid)
        {
            return this.CreateProblemDetailsForModelValidation(
                "Unable to add this tech.",
                validations.ToDictionary());
        }

        var addedBy = User.Claims.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;

        var isInRole = User.IsInRole("Admin"); // something like this. TODO.

        // what is this? From a Create Tech Request, create a TechReponse
        var response = request.MapToResponse();

        var entity = response.MapToEntity();
        entity.AddedBy = addedBy;
        entity.DateAdded = timeProvider.GetUtcNow();

        session.Store(entity);
        await session.SaveChangesAsync(token);

        return Created($"/techs/{response.Id}", response);
    }


    /// <summary>
    /// This will allow you to get information about a tech.
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}")] // techs/{guid}
    [SwaggerOperation(Tags = ["Techs"])]
    public async Task<ActionResult<TechResponse>> GetByIdAsync(Guid id, CancellationToken token)
    {
        // Marten code. Your code goes here.
        var entity = await session.Query<TechEntity>()
            .Where(t => t.Id == id)
            .ProjectToResponse()
            .SingleOrDefaultAsync(token: token);

        if (entity is null)
        {
            return NotFound();
        }
        else
        {
            return Ok(entity);
        }
    }
    [HttpGet]
    [SwaggerOperation(Tags = ["Techs"])]
    public async Task<ActionResult> GetAllTechs(
       CancellationToken token,
       [FromQuery] string? email = null
        )
    {
        IQueryable<TechEntity> techs = session.Query<TechEntity>();
        if (email is not null)
        {
            techs = techs.Where(t => t.Email == email);
        }


        var response = await techs.ToListAsync(token);
        return Ok(new { data = response, count = response.Count });

    }
}




