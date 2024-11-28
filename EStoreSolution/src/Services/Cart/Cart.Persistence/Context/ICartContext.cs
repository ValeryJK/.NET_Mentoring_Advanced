namespace Cart.Persistence.Context
{
    public interface ICartContext
    {
        IMongoCollection<Domain.Entities.Cart> Carts { get; }
    }
}
