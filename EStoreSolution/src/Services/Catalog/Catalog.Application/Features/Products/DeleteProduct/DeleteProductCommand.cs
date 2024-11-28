using Catalog.Application.Validation;
using Catalog.Domain.Interfaces;
using FluentResults;
using MediatR;

namespace Catalog.Application.Features.Products.DeleteProduct
{
    public class DeleteProductCommand : IRequest<Result<Unit>>
    {
        public int Id { get; set; }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result<Unit>>
    {
        private readonly IProductRepository productRepository;

        public DeleteProductCommandHandler(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public async Task<Result<Unit>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await this.productRepository.GetByIdAsync(request.Id);

            if (product == null)
            {
                return Result.Fail(new NotFoundError($"Product with id {request.Id} cannot be found"));
            }

            await this.productRepository.DeleteAsync(product);

            return Result.Ok(Unit.Value);
        }
    }
}