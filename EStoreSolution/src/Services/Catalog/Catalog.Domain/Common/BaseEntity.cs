using System.ComponentModel.DataAnnotations;

namespace Catalog.Domain.Common
{
	public abstract class BaseEntity
	{
		[Key]
		public int Id { get; set; }
	}
}
