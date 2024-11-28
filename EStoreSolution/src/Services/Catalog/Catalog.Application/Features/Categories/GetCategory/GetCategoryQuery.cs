using Catalog.Application.Validation;
using Catalog.Domain.Interfaces;
using FluentResults;
using Mapster;
using MediatR;

namespace Catalog.Application.Features.Categories.GetCategory
{
    public class GetCategoryQuery : IRequest<Result<GetCategoryQueryResponse>>
    {
        public int Id { get; set; }
    }

    public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, Result<GetCategoryQueryResponse>>
    {
        private readonly ICategoryRepository categoryRepository;

        public GetCategoryQueryHandler(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public async Task<Result<GetCategoryQueryResponse>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
        {
            var category = await this.categoryRepository.GetByIdAsync(request.Id);

            if (category is null)
            {
                return Result.Fail(new NotFoundError($"Category with id {request.Id} cannot be found"));
            }

            var response = category.Adapt<GetCategoryQueryResponse>();

            return Result.Ok(response);
        }
    }
}
