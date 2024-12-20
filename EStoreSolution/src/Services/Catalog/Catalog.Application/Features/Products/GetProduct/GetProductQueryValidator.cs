﻿using FluentValidation;

namespace Catalog.Application.Features.Products.GetProduct
{
	public class GetProductQueryValidator : AbstractValidator<GetProductQuery>
	{
		public GetProductQueryValidator()
		{
			RuleFor(p => p.Id)
				.NotNull();
		}
	}
}