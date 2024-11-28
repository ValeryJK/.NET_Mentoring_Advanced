﻿using FluentValidation;

namespace Catalog.Application.Features.Products.DeleteProduct
{
    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            this.RuleFor(p => p.Id)
                .NotNull();
        }
    }
}
