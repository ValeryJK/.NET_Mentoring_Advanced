using System.Reflection;
using Catalog.API.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Extensions
{
    public static class HateoasExtensions
    {
        public static Resource<T> ToHateoasResource<T>(
            this T data,
            ControllerBase controller,
            Dictionary<string, (string ActionName, string Method)> links,
            string routeIdName = "id")
        {
            var resource = new Resource<T>(data);

            var idProperty = data?.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
            var idValue = idProperty?.GetValue(data);

            var version = controller.RouteData.Values["version"] ??
                          controller.HttpContext.GetRequestedApiVersion()?.ToString();

            foreach (var link in links)
            {
                var routeValues = new RouteValueDictionary(new { version });
                routeValues[routeIdName] = idValue;

                var url = controller.Url.Link(link.Value.ActionName, routeValues) ?? string.Empty;

                resource.AddLink(url, link.Key, link.Value.Method);
            }

            return resource;
        }
    }
}