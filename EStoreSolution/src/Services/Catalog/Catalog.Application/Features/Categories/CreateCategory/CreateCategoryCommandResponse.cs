namespace Catalog.Application.Features.Categories.CreateCategory
{
    public class CreateCategoryCommandResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public string? Image { get; set; }
    }
}
