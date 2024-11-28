namespace Catalog.Persistence.Initialization
{
    public interface IDatabaseInitializer
    {
        Task Initialize(CancellationToken cancellationToken);
    }
}
