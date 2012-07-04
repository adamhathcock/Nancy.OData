using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Linq2Rest.Parser;

namespace Nancy.OData
{
    public static class ODataExtensions
    {
        private const string ODATA_URI_KEY = "OData_Uri";

        private static NameValueCollection ParseUriOptions(NancyContext context)
        {
            object item;
            if (context.Items.TryGetValue(ODATA_URI_KEY, out item))
            {
                return item as NameValueCollection;
            }
            NameValueCollection nv = new NameValueCollection();
            context.Items.Add(ODATA_URI_KEY, nv);
            var queryString = context.Request.Url.Query;
            if (string.IsNullOrWhiteSpace(queryString))
            {
                return nv;
            }
            if (!queryString.StartsWith("?"))
            {
                throw new InvalidOperationException("Invalid OData query string " + queryString);
            }
            var parameters = queryString.Substring(1).Split('&', '=');
            if (parameters.Length % 2 != 0)
            {
                throw new InvalidOperationException("Invalid OData query string " + queryString);
            }
            for (int i = 0; i < parameters.Length; i += 2)
            {
                nv.Add(parameters[i], Uri.UnescapeDataString(parameters[i + 1]));
            }
            return nv;
        }
        public static IEnumerable<object> ApplyODataUriFilter<T>(this NancyContext context, IEnumerable<T> modelItems)
        {
            var nv = ParseUriOptions(context);

            var parser = new ParameterParser<T>();
            var filter = parser.Parse(nv);
            return filter.Filter(modelItems);
        }

        public static Response AsOData<T>(this IResponseFormatter formatter, IEnumerable<T> modelItems, HttpStatusCode code = HttpStatusCode.OK)
        {
            bool isJson = formatter.Context.Request.Headers.Accept.Select(x => x.Item1).Where(x => x.StartsWith("application/json", StringComparison.InvariantCultureIgnoreCase)).Any();

            var nv = ParseUriOptions(formatter.Context);
            string value = nv.Get("$format");
            if (string.Compare(value, "json", true) == 0)
            {
                isJson = true;
            }

            if (isJson)
            {
                return formatter.AsJson(formatter.Context.ApplyODataUriFilter(modelItems), code);
            }
            throw new NotImplementedException("Atom feeds not implemented");
        }
    }
}
