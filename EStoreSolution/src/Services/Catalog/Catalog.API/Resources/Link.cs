﻿namespace Catalog.API.Resources
{
    public class Link
    {
        public string Href { get; set; }

        public string Rel { get; set; }

        public string Method { get; set; }

        public Link(string href, string rel, string method)
        {
            this.Href = href;
            this.Rel = rel;
            this.Method = method;
        }
    }
}
