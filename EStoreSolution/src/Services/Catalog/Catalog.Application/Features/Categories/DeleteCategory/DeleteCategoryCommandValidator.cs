using FluentValidation;

namespace Catalog.Application.Features.Categories.DeleteCategory
{
	public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
	{
		public DeleteCategoryCommandValidator()
		{
			RuleFor(p => p.Id)
				.NotNull();
		}
	}
}