using Catalog.Domain.Interfaces;
using FluentResults;
using MediatR;
using Mapster;

namespace Catalog.Application.Features.Categories.GetCategories
{
	public class GetCategoriesQuery : IRequest<Result<IEnumerable<GetCategoriesQueryResponse>>>
	{
	}

	public class GetCategoriesListQueryHandler : IRequestHandler<GetCategoriesQuery, Result<IEnumerable<GetCategoriesQueryResponse>>>
	{
		private readonly ICategoryRepository _categoryRepository;

		public GetCategoriesListQueryHandler(ICategoryRepository categoryRepository)
		{
			_categoryRepository = categoryRepository;
		}

		public async Task<Result<IEnumerable<GetCategoriesQueryResponse>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
		{
			var categories = (await _categoryRepository.ListAllAsync()).OrderBy(x => x.Name);
			var response = categories.Adapt<IEnumerable<GetCategoriesQueryResponse>>();

			return Result.Ok(response);
		}
	}
}
