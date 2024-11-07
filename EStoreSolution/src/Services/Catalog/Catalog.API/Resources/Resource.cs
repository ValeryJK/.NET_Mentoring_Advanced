namespace Catalog.API.Resources
{
	public class Resource<T>
	{
		public T Data { get; set; }
		public List<Link> Links { get; set; } = new();

		public Resource(T data)
		{
			Data = data;
		}

		public void AddLink(string href, string rel, string method)
		{
			Links.Add(new Link(href, rel, method));
		}
	}
}
