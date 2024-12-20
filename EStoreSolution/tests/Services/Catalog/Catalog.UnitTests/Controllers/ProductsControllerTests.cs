﻿using Catalog.API.Controllers;
using Catalog.Application.Features.Products.GetProducts;
using Catalog.Domain.Common;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Catalog.UnitTests.Controllers
{
	public class ProductsControllerTests
	{
		private readonly ProductsController _controller;
		private readonly Mock<IMediator> _mediatorMock;

		public ProductsControllerTests()
		{
			_mediatorMock = new Mock<IMediator>();
			_controller = new ProductsController(_mediatorMock.Object);
		}

		[Fact]
		public async Task GetPagedProducts_ReturnsOkResult_WithPagedProducts()
		{
			// Arrange
			var products = new List<GetProductsQueryResponse>
		{
			new GetProductsQueryResponse { Id = 1, Name = "Laptop", Price = 1000 },
			new GetProductsQueryResponse { Id = 2, Name = "Smartphone", Price = 500 }
		};

			var pagedResponse = new PagedResponse<GetProductsQueryResponse>(
				data: products,
				pageNumber: 1,
				pageSize: 10,
				totalCount: 2
			);

			var result = Result.Ok(pagedResponse);

			_mediatorMock
				.Setup(m => m.Send(It.IsAny<GetPagedProductsQuery>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(result);

			// Act
			var actionResult = await _controller.GetPagedProducts();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(actionResult);
			var returnValue = Assert.IsType<PagedResponse<GetProductsQueryResponse>>(okResult.Value);
			Assert.Equal(2, returnValue.Data.Count());
		}
	}
}
