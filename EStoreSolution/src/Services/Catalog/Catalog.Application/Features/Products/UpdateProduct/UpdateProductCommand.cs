using Catalog.Application.Validation;
using Catalog.Domain.Interfaces;
using Contracts.Messages;
using FluentResults;
using Mapster;
using MassTransit;
using MediatR;

namespace Catalog.Application.Features.Products.UpdateProduct
{
	public class UpdateProductCommand : IRequest<Result<UpdateProductCommandResponse>>
	{
		public int Id { get; set; }
		public string Name { get; set; } = default!;
		public string? Description { get; set; }
		public string? Image { get; set; }
		public decimal Price { get; set; }
		public int Amount { get; set; }
		public int CategoryId { get; set; }
	}

	public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<UpdateProductCommandResponse>>
	{
		private readonly IProductRepository _productRepository;
		private readonly ICategoryRepository _categoryRepository;
		private readonly IPublishEndpoint _publishEndpoint;

		public UpdateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository, IPublishEndpoint publishEndpoint)
		{
			_productRepository = productRepository;
			_categoryRepository = categoryRepository;
			_publishEndpoint = publishEndpoint;
		}

		public async Task<Result<UpdateProductCommandResponse>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
		{
			var product = await _productRepository.GetByIdAsync(request.Id);
			if (product is null)
			{
				return Result.Fail(new NotFoundError($"Product with id {request.Id} cannot be found"));
			}

			var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
			if (category is null)
			{
				return Result.Fail(new NotFoundError($"Category with id {request.CategoryId} cannot be found"));
			}

			request.Adapt(product);
			product.Category = category;

			await _productRepository.UpdateAsync(product);
			var response = product.Adapt<UpdateProductCommandResponse>();

			var updateEvent = new UpdateCartItemEvent
			{
				Id = request.Id,
				Price = request.Price,
				Name = request.Name
			};

			await _publishEndpoint.Publish(updateEvent);

			return Result.Ok(response);
		}
	}
}