using Catalog.Application.Validation;
using Catalog.Domain.Interfaces;
using FluentResults;
using MediatR;

namespace Catalog.Application.Features.Categories.DeleteCategory
{
    public class DeleteCategoryCommand : IRequest<Result<Unit>>
    {
        public int Id { get; set; }
    }

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result<Unit>>
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IProductRepository productRepository;

        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            this.categoryRepository = categoryRepository;
            this.productRepository = productRepository;
        }

        public async Task<Result<Unit>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await this.categoryRepository.GetByIdAsync(request.Id);
            if (category is null)
            {
                return Result.Fail(new NotFoundError($"Category with id {request.Id} cannot be found"));
            }

            var products = await this.productRepository.GetByCategoryIdAsync(request.Id);
            foreach (var product in products)
            {
                await this.productRepository.DeleteAsync(product);
            }

            await this.categoryRepository.DeleteAsync(category);

            return Result.Ok(Unit.Value);
        }
    }
}
