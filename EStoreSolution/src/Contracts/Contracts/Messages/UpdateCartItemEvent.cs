namespace Contracts.Messages
{
	public class UpdateCartItemEvent
	{
		public int Id { get; set; }
		public decimal Price { get; set; }
		public required string Name { get; set; }
	}
}