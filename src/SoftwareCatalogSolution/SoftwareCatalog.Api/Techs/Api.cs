

using JasperFx.Core;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Riok.Mapperly.Abstractions;
using System.Security.Claims;

namespace SoftwareCatalog.Api.Techs;

public class Api : ControllerBase
{
    [Authorize]
    [HttpPost("/techs")]
    public async Task<ActionResult> AddATechAsync(
        [FromBody] CreateTechRequest request,
        [FromServices] IValidator<CreateTechRequest> validator,
        [FromServices] IDocumentSession session,
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
        await session.SaveChangesAsync();

        return Created($"/techs/{response.Id}", response);
    }

    [HttpGet("/techs/{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid Id, [FromServices] IDocumentSession session, CancellationToken token)
    {
        // Marten code. Your code goes here.
        var entity = await session.Query<TechEntity>()
            .Where(t => t.Id == Id)
            .Select(t => new TechResponse
            {
                Id = t.Id,
                FirstName = t.FirstName,
                LastName = t.LastName,
                Email = t.Email,
                Phone = t.Phone
            })
            .SingleOrDefaultAsync();

        if (entity is null)
        {
            return NotFound();
        }
        else
        {
            return Ok(entity);
        }
    }
    [HttpGet("/techs")]
    public async Task<ActionResult> GetAllTechs(

        [FromServices] IDocumentSession session, CancellationToken token,
         [FromQuery] string? email = null
        )
    {
        IQueryable<TechEntity> techs;
        if (email is null)
        {
            techs = session.Query<TechEntity>().AsQueryable();
        }
        else
        {
            techs = session.Query<TechEntity>().Where(t => t.Email == email);
        }

        var response = await techs.ToListAsync();
        return Ok(new { techs = response });
    }
}


public record CreateTechRequest
{

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public TechResponse MapToResponse()
    {
        return new TechResponse
        {
            Id = Guid.NewGuid(),
            FirstName = FirstName,
            LastName = LastName,
            Email = Email,
            Phone = Phone,
        };
    }
}

public record TechResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

[Mapper]
public static partial class TechMappers
{
    public static partial TechEntity MapToEntity(this TechResponse response);
    //  public static partial IQueryable<TechResponse> ProjectToResponse(this TechEntity entity);

}

public class CreateTechRequestValidator : AbstractValidator<CreateTechRequest>
{
    public CreateTechRequestValidator(IDocumentSession session)
    {
        RuleFor(c => c.FirstName).NotEmpty();
        RuleFor(c => c.LastName).NotEmpty().MinimumLength(3).MaximumLength(20);
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.Phone).NotEmpty().WithMessage("Give us a company phone number, please");
        RuleFor(c => c.Email).MustAsync(async (email, cancellation) =>
        {
            var exists = await session.Query<TechEntity>().AnyAsync(t => t.Email == email, cancellation);
            return !exists;
        }).WithMessage("Another Tech Is Using that Email Address");
    }
}

/*
 *  Operation: Create a Tech

 * Operands:
    - We need their name (first, last),
    - we need their email address
    - We need their phone number
    - we need their "identifier" (sub claim) (more on this later)*/