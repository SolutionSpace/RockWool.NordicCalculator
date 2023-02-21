using System;
using System.Configuration;
using System.Web;
using System.Linq;

namespace REDAirCalculator.Utilities
{
    public static class UrlHelper
    {
        public static string GetUrlCulture(HttpRequestBase request)
        {
            Uri uri = request.UrlReferrer == null ? request.Url : request.UrlReferrer;

            bool isLocal = request.IsLocal;
            bool isDev = Convert.ToBoolean(ConfigurationManager.AppSettings["isDev"]);

            // used on production and on localhost
            if (!isDev || isLocal)
            {
                string topLevelDomain = uri.AbsolutePath.Contains("/en/") ? "en" : GetTopLevelDomain(uri);
                return ConvertToCulture(topLevelDomain);
            }

            string[] segments = uri.Segments;
            string culture = segments.Count() <= 2 ? "en" : segments[1].Replace("/", "");
            return ConvertToCulture(culture);
        }

        public static string GetTopLevelDomain(Uri uri)
        {
            string host = uri.Host;
            int hostIndex = host.LastIndexOf(".");
            return host.Substring(hostIndex + 1);
        }

        public static string ConvertToCulture(string topLevelDomain)
        {
            if (topLevelDomain == "se")
            {
                topLevelDomain = "sv";
            }

            if (topLevelDomain == "dk")
            {
                topLevelDomain = "da";
            }

            return topLevelDomain;
        }

        public static string GetDomainUrl(string request)
        {
            Uri uri = new Uri(request);

            return $"{uri.Scheme}://{uri.Authority}";
        }

        public static string GetRedirectUrl(string request, string auth0RedirectUri)
        {
            string url = GetDomainUrl(request);

            return $"{url}{auth0RedirectUri}";
        }
    }

}