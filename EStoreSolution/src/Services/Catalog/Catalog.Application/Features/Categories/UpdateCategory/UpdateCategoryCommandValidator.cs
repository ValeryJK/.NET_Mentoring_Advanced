using FluentValidation;

namespace Catalog.Application.Features.Categories.UpdateCategory
{
    public class ChangeNameCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public ChangeNameCommandValidator()
        {
            this.RuleFor(p => p.Id)
                .NotNull();

            this.RuleFor(p => p.Name)
                .NotEmpty();
        }
    }
}