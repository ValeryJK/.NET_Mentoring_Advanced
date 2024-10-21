namespace Cart.Domain.Common
{
	public class DatabaseSettings
	{
		public string ConnectionString { get; set; } = string.Empty;
		public string DatabaseName { get; set; } = string.Empty;
		public string CartName { get; set; } = string.Empty;
	}
}
