using Catalog.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.Domain.Entities
{
	public class Category: BaseEntity
	{
		[Required]
		[MaxLength(50)]
		public required string Name { get; set; }

		[Url]
		public string? Image { get; set; }    

		public int? ParentCategoryId { get; set; }

		[ForeignKey("ParentCategoryId")]
		public virtual Category? ParentCategory { get; set; }

		public virtual ICollection<Category> ChildCategories { get; set; } = new List<Category>();

		public virtual ICollection<Product> Products { get; set; } = new List<Product>();
	}
}
