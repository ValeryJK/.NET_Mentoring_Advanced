namespace Catalog.API.Resources
{
    public class HateoasLinks
    {
        private readonly Dictionary<string, (string ActionName, string Method)> links;

        public HateoasLinks()
        {
            this.links = new Dictionary<string, (string ActionName, string Method)>();
        }

        public HateoasLinks AddLink(string rel, string actionName, string method)
        {
            this.links[rel] = (actionName, method);
            return this;
        }

        public Dictionary<string, (string ActionName, string Method)> Build() => this.links;
    }
}