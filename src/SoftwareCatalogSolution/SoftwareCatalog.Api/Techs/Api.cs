using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SoftwareCatalog.Api.Shared;

namespace SoftwareCatalog.Api.Techs;

public class Api : ControllerBase
{

    [HttpPost("/techs")]
    public async Task<ActionResult> AddATechAsync(
        [FromBody] CreateTechRequest request,
        [FromServices] IValidator<CreateTechRequest> validator,
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

        // Is the person making the request allowed to make this request (Authn/Authz) Later.
        // did they send us the right data?
        //   if not, send them an error.
        // If they did, do we want this person as a tech?
        // - Business rules, etc....
        // If so,create a new subordinate resource.

        var response = new TechResponse
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
        };
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