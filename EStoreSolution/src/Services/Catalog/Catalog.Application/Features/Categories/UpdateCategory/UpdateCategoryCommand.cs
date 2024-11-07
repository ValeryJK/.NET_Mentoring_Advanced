using Catalog.Application.Validation;
using Catalog.Domain.Interfaces;
using FluentResults;
using MediatR;

namespace Catalog.Application.Features.Categories.UpdateCategory
{
	public class UpdateCategoryCommand : IRequest<Result<Unit>>
	{
		public int Id { get; set; }
		public required string Name { get; set; }
		public required string Image { get; set; }
	}

	public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<Unit>>
	{
		private readonly ICategoryRepository _categoryRepository;

		public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
		{
			_categoryRepository = categoryRepository;
		}

		public async Task<Result<Unit>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
		{
			var category = await _categoryRepository.GetByIdAsync(request.Id);
			if (category == null)
			{
				return Result.Fail(new NotFoundError($"Category with id {request.Id} cannot be found"));
			}

			category.Name = request.Name;
			category.Image = request.Image;
			await _categoryRepository.UpdateAsync(category);

			return Result.Ok(Unit.Value);
		}
	}
}
