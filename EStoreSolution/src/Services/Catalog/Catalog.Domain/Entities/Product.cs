using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Catalog.Domain.Common;

namespace Catalog.Domain.Entities
{
    public class Product : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }

        [DataType(DataType.Html)]
        public string? Description { get; set; }

        [Url]
        public string? Image { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be a positive integer")]
        public int Amount { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
    }
}
