using FluentValidation;

namespace Catalog.Application.Features.Products.CreateProduct
{
	public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
	{
		public CreateProductCommandValidator()
		{
			RuleFor(p => p.Name)
				.NotEmpty();

			RuleFor(p => p.CategoryId)
				.NotNull();

			RuleFor(p => p.Price)
				.NotNull()
				.GreaterThanOrEqualTo(0);
		}
	}
}