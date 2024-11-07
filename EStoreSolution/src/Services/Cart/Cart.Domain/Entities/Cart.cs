using MongoDB.Bson.Serialization.Attributes;

namespace Cart.Domain.Entities
{
	public class Cart
	{
		[BsonId]
		public string Id { get; set; } = string.Empty;
		public List<CartItem> CartItems { get; set; } = new();
	}
}
