using System;
using System.Web.Caching;
using System.Web.Mvc;

namespace REDAirCalculator.Utilities
{
    public static class FilesVersioning
    {
        public static MvcHtmlString VersionedFile(this HtmlHelper helper, string filename)
        {
            string version = GetVersion(helper, filename);
            string htmlPath = filename + version;

            return MvcHtmlString.Create(htmlPath.Contains(".css") ? $"<link rel='stylesheet' href='{htmlPath}'/>" : $"<script type='text/javascript' src='{htmlPath}'></script>");
        }

        private static string GetVersion(this HtmlHelper helper, string filename)
        {
            var context = helper.ViewContext.RequestContext.HttpContext;

            if (context.Cache[filename] == null)
            {
                var physicalPath = context.Server.MapPath(filename);
                var version = $"?v={new System.IO.FileInfo(physicalPath).LastWriteTime:MMddHHmmss}";
                context.Cache.Add(filename, version, null,
                    DateTime.Now.AddMinutes(5), TimeSpan.Zero,
                    CacheItemPriority.Normal, null);
                return version;
            }
            else
            {
                return context.Cache[filename] as string;
            }
        }
    }
}