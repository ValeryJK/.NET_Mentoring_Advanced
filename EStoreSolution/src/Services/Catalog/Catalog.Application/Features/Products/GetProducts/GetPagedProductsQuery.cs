using Catalog.Domain.Common;
using Catalog.Domain.Interfaces;
using FluentResults;
using Mapster;
using MediatR;

namespace Catalog.Application.Features.Products.GetProducts
{

	public class GetPagedProductsQuery : IRequest<Result<PagedResponse<GetProductsQueryResponse>>>
	{
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public int? CategoryId { get; set; }
	}

	public class GetPagedProductsQueryHandler : IRequestHandler<GetPagedProductsQuery, Result<PagedResponse<GetProductsQueryResponse>>>
	{
		private readonly IProductRepository _productRepository;

		public GetPagedProductsQueryHandler(IProductRepository productRepository)
		{
			_productRepository = productRepository;
		}

		public async Task<Result<PagedResponse<GetProductsQueryResponse>>> Handle(GetPagedProductsQuery request, CancellationToken cancellationToken)
		{
			var pagedProducts = await _productRepository.GetProductsPagedAsync(request.PageNumber, request.PageSize, request.CategoryId);

			var pagedResponse = new PagedResponse<GetProductsQueryResponse>
			(
				data: pagedProducts.Data.Adapt<IEnumerable<GetProductsQueryResponse>>(),
				pageNumber: pagedProducts.PageNumber,
				pageSize: pagedProducts.PageSize,
				totalCount: pagedProducts.TotalCount
			);

			return Result.Ok(pagedResponse);
		}
	}
}