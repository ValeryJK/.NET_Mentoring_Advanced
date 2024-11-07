using FluentValidation;

namespace Catalog.Application.Features.Categories.GetCategory
{
	public class GetCategoryQueryValidator : AbstractValidator<GetCategoryQuery>
	{
		public GetCategoryQueryValidator()
		{
			RuleFor(p => p.Id)
				.NotNull();
		}
	}
}