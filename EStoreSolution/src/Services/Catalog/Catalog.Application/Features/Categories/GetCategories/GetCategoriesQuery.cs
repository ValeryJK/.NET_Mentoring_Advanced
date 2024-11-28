using Catalog.Domain.Interfaces;
using FluentResults;
using Mapster;
using MediatR;

namespace Catalog.Application.Features.Categories.GetCategories
{
    public class GetCategoriesQuery : IRequest<Result<IEnumerable<GetCategoriesQueryResponse>>>
    {
    }

    public class GetCategoriesListQueryHandler : IRequestHandler<GetCategoriesQuery, Result<IEnumerable<GetCategoriesQueryResponse>>>
    {
        private readonly ICategoryRepository categoryRepository;

        public GetCategoriesListQueryHandler(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public async Task<Result<IEnumerable<GetCategoriesQueryResponse>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = (await this.categoryRepository.ListAllAsync()).OrderBy(x => x.Name);
            var response = categories.Adapt<IEnumerable<GetCategoriesQueryResponse>>();

            return Result.Ok(response);
        }
    }
}
