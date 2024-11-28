﻿namespace Catalog.Application.Features.Products.UpdateProduct
{
    public class UpdateProductCommandResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        public string? Image { get; set; }

        public decimal Price { get; set; }

        public int Amount { get; set; }

        public int CategoryId { get; set; }
    }
}