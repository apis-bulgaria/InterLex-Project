namespace NewInterlex.Api.Validation
{
    using FluentValidation;
    using Models.Request;

    public class DataInsertLinksRequestValidator : AbstractValidator<SaveGraphDataRequest>
    {
        public DataInsertLinksRequestValidator()
        {
            this.RuleFor(x => x.Content).NotEmpty();
        }
    }
}