using MongoDB.Bson.Serialization.Attributes;

namespace Cart.Domain.Entities
{
	public class Cart
	{
		[BsonId]
		public Guid Id { get; set; }
		public List<CartItem> CartItems { get; set; } = new();
	}
}
