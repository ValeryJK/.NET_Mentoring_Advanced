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
		private readonly ICategoryRepository _categoryRepository;
		private readonly IProductRepository _productRepository;

		public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IProductRepository productRepository)
		{
			_categoryRepository = categoryRepository;
			_productRepository = productRepository;
		}

		public async Task<Result<Unit>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
		{
			var category = await _categoryRepository.GetByIdAsync(request.Id);
			if (category is null)
			{
				return Result.Fail(new NotFoundError($"Category with id {request.Id} cannot be found"));
			}

			var products = await _productRepository.GetByCategoryIdAsync(request.Id);
			foreach (var product in products)
				await _productRepository.DeleteAsync(product);

			await _categoryRepository.DeleteAsync(category);

			return Result.Ok(Unit.Value);
		}
	}
}
