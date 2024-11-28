using Cart.Domain.Entities;

namespace Cart.Persistence.Initialization
{
    public static class CartContextSeed
    {
        public static void SeedData(IMongoCollection<Domain.Entities.Cart> cartCollection)
        {
            bool existCart = cartCollection.Find(p => true).Any();
            if (!existCart)
            {
                cartCollection.InsertManyAsync(GetPreconfiguredCarts());
            }
        }

        private static IEnumerable<Domain.Entities.Cart> GetPreconfiguredCarts()
        {
            return new List<Domain.Entities.Cart>()
            {
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    CartItems = new List<CartItem>
                    {
                        new CartItem
                        {
                            Id = 1,
                            Name = "CartItem 1",
                            Price = 10.99m,
                            Quantity = 2
                        },
                        new CartItem
                        {
                            Id = 2,
                            Name = "CartItem 2",
                            Price = 20.49m,
                            Quantity = 1
                        },
                        new CartItem
                        {
                            Id = 3,
                            Name = "CartItem 3",
                            Price = 30.49m,
                            Quantity = 4
                        },
                    }
                }
            };
        }
    }
}
