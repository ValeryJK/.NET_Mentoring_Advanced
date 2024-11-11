using Catalog.Application.Validation;
using Catalog.Domain.Interfaces;
using FluentResults;
using Mapster;
using MediatR;

namespace Catalog.Application.Features.Products.GetProduct
{
	public class GetProductQuery : IRequest<Result<GetProductQueryResponse>>
	{
		public int Id { get; set; }
	}

	public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Result<GetProductQueryResponse>>
	{
		private readonly IProductRepository _productRepository;

		public GetProductQueryHandler(IProductRepository productRepository)
		{
			_productRepository = productRepository;
		}

		public async Task<Result<GetProductQueryResponse>> Handle(GetProductQuery request, CancellationToken cancellationToken)
		{
			var product = await _productRepository.GetByIdAsync(request.Id);
			if (product is null)
			{
				return Result.Fail(new NotFoundError($"Product with ID {request.Id} cannot be found"));
			}

			var response = product.Adapt<GetProductQueryResponse>();
			return Result.Ok(response);
		}
	}
}