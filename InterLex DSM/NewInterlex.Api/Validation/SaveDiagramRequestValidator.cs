namespace NewInterlex.Api.Validation
{
    using FluentValidation;
    using Models.Request;

    public class SaveDiagramRequestValidator : AbstractValidator<SaveGraphRequest>
    {
        public SaveDiagramRequestValidator()
        {
            this.RuleFor(x => x.Content).NotEmpty();
        }
    }
}