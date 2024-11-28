namespace Catalog.Persistence.Initialization.Seed
{
    public interface ICustomSeedRunner
    {
        Task RunSeeders(bool isDevelopment);
    }
}
