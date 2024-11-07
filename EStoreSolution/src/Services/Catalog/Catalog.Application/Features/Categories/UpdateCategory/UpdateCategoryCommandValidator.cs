using FluentValidation;

namespace Catalog.Application.Features.Categories.UpdateCategory
{
	public class ChangeNameCommandValidator : AbstractValidator<UpdateCategoryCommand>
	{
		public ChangeNameCommandValidator()
		{
			RuleFor(p => p.Id)
				.NotNull();

			RuleFor(p => p.Name)
				.NotEmpty();
		}
	}
}