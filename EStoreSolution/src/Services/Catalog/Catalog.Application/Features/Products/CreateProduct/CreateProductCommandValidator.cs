using FluentValidation;

namespace Catalog.Application.Features.Products.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            this.RuleFor(p => p.Name)
                .NotEmpty();

            this.RuleFor(p => p.CategoryId)
                .NotNull();

            this.RuleFor(p => p.Price)
                .NotNull()
                .GreaterThanOrEqualTo(0);
        }
    }
}