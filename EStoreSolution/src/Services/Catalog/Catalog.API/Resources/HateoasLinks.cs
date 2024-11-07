namespace Catalog.API.Resources
{
	public class HateoasLinks
	{
		private readonly Dictionary<string, (string ActionName, string Method)> _links;

		public HateoasLinks()
		{
			_links = new Dictionary<string, (string ActionName, string Method)>();
		}

		public HateoasLinks AddLink(string rel, string actionName, string method)
		{
			_links[rel] = (actionName, method);
			return this;
		}

		public Dictionary<string, (string ActionName, string Method)> Build() => _links;
	}
}