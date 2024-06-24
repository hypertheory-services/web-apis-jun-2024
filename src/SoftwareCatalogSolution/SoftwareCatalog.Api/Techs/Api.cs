

using Marten;

namespace SoftwareCatalog.Api.Techs;

public class Api : ControllerBase
{

    [HttpPost("/techs")]
    public async Task<ActionResult> AddATechAsync(
        [FromBody] CreateTechRequest request,
        [FromServices] IValidator<CreateTechRequest> validator,
        [FromServices] IDocumentSession session,
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




        var response = new TechResponse
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
        };

        var entity = new TechEntity
        {
            Id = response.Id,
            FirstName = response.FirstName,
            LastName = response.LastName,
            Email = response.Email,
            Phone = response.Phone,
            DateAdded = DateTimeOffset.UtcNow
        };

        session.Insert(entity);
        await session.SaveChangesAsync();

        return Created($"/techs/{response.Id}", response);
    }
}


public record CreateTechRequest
{

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public record TechResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class CreateTechRequestValidator : AbstractValidator<CreateTechRequest>
{
    public CreateTechRequestValidator()
    {
        RuleFor(c => c.FirstName).NotEmpty();
        RuleFor(c => c.LastName).NotEmpty().MinimumLength(3).MaximumLength(20);
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.Phone).NotEmpty().WithMessage("Give us a company phone number, please");
    }
}

/*
 *  Operation: Create a Tech

 * Operands:
    - We need their name (first, last),
    - we need their email address
    - We need their phone number
    - we need their "identifier" (sub claim) (more on this later)*/