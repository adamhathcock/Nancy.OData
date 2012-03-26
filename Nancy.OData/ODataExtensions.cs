using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Linq2Rest.Parser;

namespace Nancy.OData
{
    public static class ODataExtensions
    {
        public static IEnumerable<object> ODataFilter<T>(this NancyContext context, IEnumerable<T> modelItems)
        {
            var queryString = context.Request.Url.Query;
            if (!queryString.StartsWith("?"))
            {
                throw new InvalidOperationException("Invalid OData query string " + queryString);
            }
            var parameters = queryString.Substring(1).Split('&', '=');
            var nv = new NameValueCollection();
            if (parameters.Length % 2 != 0)
            {
                throw new InvalidOperationException("Invalid OData query string " + queryString);
            }
            for (int i = 0; i < parameters.Length; i += 2)
            {
                nv.Add(parameters[i], parameters[i + 1]);
            }

            var parser = new ParameterParser<T>();
            var filter = parser.Parse(nv);
            return filter.Filter(modelItems);
        }

        public static Response AsOData<T>(this IResponseFormatter formatter, IEnumerable<T> modelItems, HttpStatusCode code = HttpStatusCode.OK)
        {
            bool isJson = formatter.Context.Request.Headers.Accept.Select(x => x.Item1).Where(x => x.StartsWith("application/json", StringComparison.InvariantCultureIgnoreCase)).Any();
            if (isJson)
            {
                return formatter.AsJson(formatter.Context.ODataFilter(modelItems), code);
            }
            return formatter.AsXml(formatter.Context.ODataFilter(modelItems));
        }
    }
}
