using Catalog.Application.Validation;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using FluentResults;
using Mapster;
using MediatR;

namespace Catalog.Application.Features.Products.CreateProduct
{
    public class CreateProductCommand : IRequest<Result<CreateProductCommandResponse>>
    {
        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        public string? Image { get; set; }

        public decimal Price { get; set; }

        public int Amount { get; set; }

        public int CategoryId { get; set; }
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<CreateProductCommandResponse>>
    {
        private readonly IProductRepository productRepository;
        private readonly ICategoryRepository categoryRepository;

        public CreateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
        }

        public async Task<Result<CreateProductCommandResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = request.Adapt<Product>();
            var category = await this.categoryRepository.GetByIdAsync(request.CategoryId);
            if (category is null)
            {
                return Result.Fail(new NotFoundError($"Category with id {request.CategoryId} cannot be found"));
            }

            product.Category = category;
            await this.productRepository.AddAsync(product);
            var response = product.Adapt<CreateProductCommandResponse>();

            return Result.Ok(response);
        }
    }
}
