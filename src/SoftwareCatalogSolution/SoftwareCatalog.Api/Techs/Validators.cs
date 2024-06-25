using Marten;

namespace SoftwareCatalog.Api.Techs;

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