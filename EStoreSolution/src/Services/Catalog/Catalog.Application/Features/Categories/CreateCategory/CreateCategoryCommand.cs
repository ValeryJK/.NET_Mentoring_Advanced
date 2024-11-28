using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using FluentResults;
using Mapster;
using MediatR;

namespace Catalog.Application.Features.Categories.CreateCategory
{
    public class CreateCategoryCommand : IRequest<Result<CreateCategoryCommandResponse>>
    {
        public string Name { get; set; } = default!;

        public string? Image { get; set; }
    }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CreateCategoryCommandResponse>>
    {
        private readonly ICategoryRepository categoryRepository;

        public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public async Task<Result<CreateCategoryCommandResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new Category
            {
                Name = request.Name,
                Image = request.Image
            };

            category = await this.categoryRepository.AddAsync(category);
            var response = category.Adapt<CreateCategoryCommandResponse>();

            return Result.Ok(response);
        }
    }
}
